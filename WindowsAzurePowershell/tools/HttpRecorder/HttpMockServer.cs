﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Utilities.HttpRecorder
{
    public class HttpMockServer : DelegatingHandler
    {
        private const string recordDir = "SessionRecords";
        private const string modeEnvironmentVariableName = "AZURE_TEST_MODE";
        private static AssetNames names;
        private static List<RecordEntry> sessionRecords;

        static HttpMockServer()
        {
            CleanRecordsDirectory = true;
            Mode = GetCurrentMode();
        }

        private HttpMockServer() { }

        public static void Initialize(IRecordMatcher matcher, Type callerIdentity)
        {
            HttpMockServer server = new HttpMockServer();
            Matcher = matcher;
            Identity = callerIdentity.Name;

            server.InitializeState();
            instance = server;
        }

        private void InitializeState()
        {
            names = new AssetNames();
            sessionRecords = new List<RecordEntry>();
            Records = new Records(Matcher);
        }

        public void Start()
        {
            if (Mode == HttpRecorderMode.Playback)
            {
                if (Directory.Exists(RecordsDirectory))
                {
                    foreach (string recordsFile in Directory.GetFiles(RecordsDirectory, "record-*.json"))
                    {
                        RecordEntryPack pack = RecordEntryPack.Deserialize(recordsFile);
                        sessionRecords.AddRange(pack.Entries);
                        foreach (var func in pack.Names.Keys)
                        {
                            pack.Names[func].ForEach(n => names.Enqueue(func, n));
                        }
                    }
                }
                Records.EnqueueRange(sessionRecords);
            }
        }
        
        private static HttpMockServer instance = null;
        public static HttpMockServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HttpMockServer();
                    instance.InitializeState();
                    instance.Start();
                }
                return instance;
            }
        }

        public static HttpRecorderMode Mode { get; set; }
        public static IRecordMatcher Matcher { get; set; }
        public static bool CleanRecordsDirectory { get; set; }
        public static string OutputDirectory { get; set; }
        public static string Identity { get; set; }

        public string RecordsDirectory
        {
            get
            {
                string dirName = Path.Combine(recordDir, Identity);
                if (Mode == HttpRecorderMode.Record)
                {
                    if (OutputDirectory != null)
                    {
                        dirName = Path.Combine(OutputDirectory, dirName);
                    }
                }
                return dirName;
            }
        }

        public Records Records { get; private set; }

        private static HttpRecorderMode GetCurrentMode()
        {
            string input =  Environment.GetEnvironmentVariable(modeEnvironmentVariableName);
            HttpRecorderMode mode;

            if (string.IsNullOrEmpty(input))
            {
                mode = HttpRecorderMode.None;
            }
            else
            {
                mode = (HttpRecorderMode)Enum.Parse(typeof(HttpRecorderMode), input, true);
            }

            return mode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (Mode == HttpRecorderMode.Playback)
            {
                // Will throw KeyNotFoundException if the request is not recorded
                return TaskEx.FromResult(Records[Matcher.GetMatchingKey(request)].Dequeue().GetResponse());
            }
            else
            {
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(response =>
                {
                    HttpResponseMessage result = response.Result;
                    if (Mode == HttpRecorderMode.Record)
                    {
                        RecordEntry recordEntry = new RecordEntry(result);
                        Records.Enqueue(new RecordEntry(result));
                        sessionRecords.Add(recordEntry);
                    }

                    return result;
                });
            }
        }

        public static string GetAssetName(string testName, string prefix)
        {
            var server = Instance;

            if (server == null)
            {
                throw new ApplicationException("HttpMockServer has not been started.");
            }

            if (Mode == HttpRecorderMode.Playback)
            {
                return server.names[testName].Dequeue();
            }
            else
            {
                string generated = prefix + new Random().Next(9999);

                if (server.names.ContainsKey(testName))
                {
                    while (server.names[testName].Any(n => n.Equals(generated)))
                    {
                        generated = prefix + new Random().Next(9999);
                    }
                }
                server.names.Enqueue(testName, generated);

                return generated;
            }
        }

        public void InjectRecordEntry(RecordEntry record)
        {
            if (Mode == HttpRecorderMode.Playback)
            {
                Records.Enqueue(record);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Mode == HttpRecorderMode.Record)
            {
                Utilities.EnsureDirectoryExists(RecordsDirectory);

                if (CleanRecordsDirectory)
                {
                    Utilities.CleanDirectory(RecordsDirectory);
                }

                RecordEntryPack pack = new RecordEntryPack();

                foreach (RecordEntry recordEntry in sessionRecords)
                {
                    pack.Entries.Add(recordEntry);
                }

                pack.Names = names.Names;

                pack.Serialize(Path.Combine(RecordsDirectory, string.Format("record-{0:yyyyMMddHHmmss}.json", DateTime.Now)));
            }

            instance = null;
        }
    }
}