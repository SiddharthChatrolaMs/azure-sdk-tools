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

namespace Microsoft.WindowsAzure.Commands.ServiceManagement.IaaS.Extensions
{
    using Model.PersistentVMModel;
    using Properties;
    using System;
    using System.Linq;
    using System.Management.Automation;

    [Cmdlet(
        VerbsCommon.Get,
        VirtualMachineExtensionNoun,
        DefaultParameterSetName = ListByExtensionParamSetName),
    OutputType(
        typeof(VirtualMachineExtensionContext))]
    public class GetAzureVMExtensionCommand : VirtualMachineExtensionCmdletBase
    {
        protected const string ListByExtensionParamSetName = "ListByExtensionName";
        protected const string ListByReferenceParamSetName = "ListByReferenceName";

        [Parameter(
            ParameterSetName = ListByExtensionParamSetName,
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Extension Name.")]
        [ValidateNotNullOrEmpty]
        public override string ExtensionName
        {
            get;
            set;
        }

        [Parameter(
            ParameterSetName = ListByExtensionParamSetName,
            Mandatory = true,
            Position = 2,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Extension Publisher.")]
        [ValidateNotNullOrEmpty]
        public override string Publisher
        {
            get;
            set;
        }

        [Parameter(
            ParameterSetName = ListByExtensionParamSetName,
            Mandatory = true,
            Position = 3,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Extension Version.")]
        [ValidateNotNullOrEmpty]
        public override string Version
        {
            get;
            set;
        }

        [Parameter(
            ParameterSetName = ListByReferenceParamSetName,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            HelpMessage = "The Extension Reference Name.")]
        [ValidateNotNullOrEmpty]
        public override string ReferenceName
        {
            get;
            set;
        }

        internal void ExecuteCommand()
        {
            var extensionRefs = GetPredicateExtensionList();
            if (extensionRefs != null)
            {
                foreach (var r in extensionRefs)
                {
                    if (r != null)
                    {
                        WriteObject(new VirtualMachineExtensionContext
                        {
                            ExtensionName = r.Name,
                            ReferenceName = r.ReferenceName,
                            Publisher = r.Publisher,
                            Version = r.Version,
                            PublicConfiguration = GetConfiguration(r, PublicTypeStr),
                            PrivateConfiguration = GetConfiguration(r, PrivateTypeStr)
                    });
                    }
                }
            }
        }

        protected override void ProcessRecord()
        {
            ServiceManagementProfile.Initialize();
            try
            {
                base.ProcessRecord();
                ExecuteCommand();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
