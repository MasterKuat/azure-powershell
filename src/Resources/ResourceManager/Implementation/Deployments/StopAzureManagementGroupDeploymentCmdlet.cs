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

using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Cmdlets.Components;
using Microsoft.Azure.Commands.ResourceManager.Cmdlets.SdkModels;
using Microsoft.Azure.Commands.ResourceManager.Cmdlets.SdkModels.Deployments;
using Microsoft.Azure.Commands.ResourceManager.Common;
using ProjectResources = Microsoft.Azure.Commands.ResourceManager.Cmdlets.Properties.Resources;

namespace Microsoft.Azure.Commands.ResourceManager.Cmdlets.Implementation
{
    /// <summary>
    /// Cancel a running deployment.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, AzureRMConstants.AzureRMPrefix + "ManagementGroupDeployment", SupportsShouldProcess = true,
        DefaultParameterSetName = StopAzureManagementGroupDeploymentCmdlet.DeploymentNameParameterSet), OutputType(typeof(bool))]
    public class StopAzureManagementGroupDeploymentCmdlet : ResourceManagerCmdletBase
    {
        /// <summary>
        /// The deployment Id parameter set.
        /// </summary>
        internal const string DeploymentIdParameterSet = "StopByDeploymentId";

        /// <summary>
        /// The deployment name parameter set.
        /// </summary>
        internal const string DeploymentNameParameterSet = "StopByDeploymentName";

        /// <summary>
        /// The input object parameter set.
        /// </summary>
        internal const string InputObjectParameterSet = "StopByInputObject";

        [Parameter(Position = 0, ParameterSetName = StopAzureManagementGroupDeploymentCmdlet.DeploymentNameParameterSet,
            Mandatory = true, HelpMessage = "The management group id.")]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupId { get; set; }

        [Alias("DeploymentName")]
        [Parameter(Position = 1, ParameterSetName = StopAzureManagementGroupDeploymentCmdlet.DeploymentNameParameterSet,
            Mandatory = true, HelpMessage = "The name of the deployment.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Alias("DeploymentId", "ResourceId")]
        [Parameter(ParameterSetName = StopAzureManagementGroupDeploymentCmdlet.DeploymentIdParameterSet,
            Mandatory = true, HelpMessage = "The fully qualified resource Id of the deployment. example: /providers/Microsoft.Management/managementGroups/{managementGroupId}/providers/Microsoft.Resources/deployments/{deploymentName}")]
        [ValidateNotNullOrEmpty]
        public string Id { get; set; }

        [Parameter(ParameterSetName = StopAzureManagementGroupDeploymentCmdlet.InputObjectParameterSet, Mandatory = true,
            ValueFromPipeline = true, HelpMessage = "The deployment object.")]
        public PSDeployment InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        protected override void OnProcessRecord()
        {
            var options = new FilterDeploymentOptions(DeploymentScopeType.ManagementGroup)
            {
                ManagementGroupId = !string.IsNullOrEmpty(this.ManagementGroupId)
                    ? this.ManagementGroupId
                    : !string.IsNullOrEmpty(this.Id) ? ResourceIdUtility.GetManagementGroupId(this.Id) : this.InputObject.ManagementGroupId,
                DeploymentName = !string.IsNullOrEmpty(this.Name)
                    ? this.Name
                    : !string.IsNullOrEmpty(this.Id) ? ResourceIdUtility.GetDeploymentName(this.Id) : this.InputObject.DeploymentName
            };

            ConfirmAction(
                ProjectResources.CancelDeploymentMessage,
                this.Name,
                () => ResourceManagerSdkClient.CancelDeployment(options));

            if (this.PassThru.IsPresent)
            {
                WriteObject(true);
            }
        }
    }
}