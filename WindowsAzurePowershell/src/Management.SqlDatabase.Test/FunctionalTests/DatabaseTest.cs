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


namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.FunctionalTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Test.Utilities;
    using Microsoft.WindowsAzure.Management.Utilities.Common.XmlSchema;
    using Microsoft.WindowsAzure.Management.Utilities.Common;

    [TestClass]
    public class DatabaseTest
    {
        private string userName;
        private string password;
        private string manageUrl;
        private string subscriptionId;
        private string serializedCert;

        /// <summary>
        /// Scripts for doing context creation tests
        /// </summary>
        private const string CreateContextScript = @"Database\CreateContext.ps1";

        /// <summary>
        /// Scripts for doing Create and Get database tests
        /// </summary>
        private const string CreateScript = @"Database\CreateAndGetDatabase.ps1";
        private const string CreateScriptWithCert = @"Database\CreateAndGetDatabaseWithCert.ps1";
        private const string CreateScriptWithServerName = @"Database\CreateAndGetDatabaseWithServerName.ps1";

        /// <summary>
        /// Scripts for duing database update tests
        /// </summary>
        private const string UpdateScript = @"Database\UpdateDatabase.ps1";
        private const string UpdateScriptWithCert = @"Database\UpdateDatabaseWithCert.ps1";

        /// <summary>
        /// Scripts for doing delete database tests
        /// </summary>
        private const string DeleteScript = @"Database\DeleteDatabase.ps1";
        private const string DeleteScriptWithCert = @"Database\DeleteDatabaseWithCert.ps1";

        /// <summary>
        /// Tests for doing format validation tests 
        /// </summary>
        private const string FormatValidationScript = @"Database\FormatValidation.ps1";

        private const string GetDatabaseScriptWithCert = @"Database\GetDatabaseWithCert";

        [TestInitialize]
        public void Setup()
        {
            XElement root = XElement.Load("SqlDatabaseSettings.xml");
            this.userName = root.Element("SqlAuthUserName").Value;
            this.password = root.Element("SqlAuthPassword").Value;
            this.manageUrl = root.Element("ManageUrl").Value;
            this.subscriptionId = root.Element("SubscriptionID").Value;
            this.serializedCert = root.Element("SerializedCert").Value;
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateContext()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-ManageUrl \"{0}\" -UserName \"{1}\" -Password \"{2}\" -SubscriptionId \"{3}\" -SerializedCert \"{4}\" ",
                this.manageUrl,
                this.userName,
                this.password,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(
                DatabaseTest.CreateContextScript,
                arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateDatabaseWithSqlAuth()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testcreatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.CreateScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateDatabaseWithCert()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -SubscriptionID \"{2}\" -SerializedCert \"{3}\"",
                "testcreatedbfromcmdlet",
                this.manageUrl,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(
                DatabaseTest.CreateScriptWithCert, 
                arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateDatabaseWithServerName()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -SubscriptionID \"{2}\" -SerializedCert \"{3}\"",
                "testcreatedbfromcmdlet",
                this.manageUrl,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(
                DatabaseTest.CreateScriptWithServerName, 
                arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void UpdateDatabaseWithSqlAuth()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testupdatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.UpdateScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void UpdateDatabaseWithCert()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -SubscriptionID \"{2}\" -SerializedCert \"{3}\"",
                "testupdatedbfromcmdlet",
                this.manageUrl,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.UpdateScriptWithCert, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void DeleteDatabaseWithSqlAuth()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testDeletedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.DeleteScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void DeleteDatabaseWithCertAuth()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -SubscriptionID \"{2}\" -SerializedCert \"{3}\"",
                "testDeletedbfromcmdlet",
                this.manageUrl,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.DeleteScriptWithCert, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void OutputObjectFormatValidation()
        {
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid() + ".txt");
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" -OutputFile \"{4}\"",
                "testFormatdbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                outputFile);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.FormatValidationScript, arguments);
            Assert.IsTrue(testResult);

            OutputFormatValidator.ValidateOutputFormat(outputFile, @"Database\ExpectedFormat.txt");
        }
    }
}
