﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
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

namespace Microsoft.WindowsAzure.Management.Websites.Cmdlets
{
    using System.Management.Automation;
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Properties;
    using Services;
    using WebSites.Cmdlets.Common;

    /// <summary>
    /// Starts an azure website.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AzureWebsite")]
    public class StartAzureWebsiteCommand : WebsiteContextCmdletBase
    {
        /// <summary>
        /// Initializes a new instance of the StartAzureWebsiteCommand class.
        /// </summary>
        public StartAzureWebsiteCommand()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StartAzureWebsiteCommand class.
        /// </summary>
        /// <param name="channel">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        public StartAzureWebsiteCommand(IWebsitesServiceManagement channel)
        {
            Channel = channel;
        }

        internal override void ExecuteCommand()
        {
            Website website = null;

            InvokeInOperationContext(() =>
            {
                try
                {
                    website = RetryCall(s => Channel.GetWebsite(s, Name));
                }
                catch (CommunicationException ex)
                {
                    WriteErrorDetails(ex);
                }
            });

            if (website == null)
            {
                throw new Exception(string.Format(Resources.InvalidWebsite, Name));
            }

            InvokeInOperationContext(() =>
            {
                try
                {
                    Website websiteUpdate = new Website
                                            {
                                                Name = Name,
                                                HostNames = new List<string> { Name + ".azurewebsites.net" },
                                                State = "Running"
                                            };

                    RetryCall(s => Channel.UpdateWebsite(s, website.WebSpace, Name, websiteUpdate));
                }
                catch (CommunicationException ex)
                {
                    WriteErrorDetails(ex);
                }
            });
        }
    }
}
