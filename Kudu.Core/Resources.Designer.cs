﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kudu.Core {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kudu.Core.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Copied &apos;{0}&apos;..
        /// </summary>
        internal static string Copied {
            get {
                return ResourceManager.GetString("Copied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to N/A.
        /// </summary>
        internal static string Deployment_UnknownValue {
            get {
                return ResourceManager.GetString("Deployment_UnknownValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to determine which project file to build. {0}..
        /// </summary>
        internal static string Error_AmbiguousProjects {
            get {
                return ResourceManager.GetString("Error_AmbiguousProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to determine which solution file to build. {0}..
        /// </summary>
        internal static string Error_AmbiguousSolutions {
            get {
                return ResourceManager.GetString("Error_AmbiguousSolutions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not fetch remote branch &apos;{0}&apos;. Verify that the branch exists in the repository..
        /// </summary>
        internal static string Error_BranchNotFound {
            get {
                return ResourceManager.GetString("Error_BranchNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to locate &apos;git.exe&apos;. Make sure it is installed..
        /// </summary>
        internal static string Error_FailedToLocateGit {
            get {
                return ResourceManager.GetString("Error_FailedToLocateGit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to locate &apos;hg.exe&apos;. Make sure it is installed..
        /// </summary>
        internal static string Error_FailedToLocateHg {
            get {
                return ResourceManager.GetString("Error_FailedToLocateHg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to locate &apos;ssh.exe&apos;. Make sure it is installed..
        /// </summary>
        internal static string Error_FailedToLocateSsh {
            get {
                return ResourceManager.GetString("Error_FailedToLocateSsh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a &apos;{0}&apos; repository but found a &apos;{1}&apos; repository at path &apos;{2}&apos;..
        /// </summary>
        internal static string Error_MismatchRepository {
            get {
                return ResourceManager.GetString("Error_MismatchRepository", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No log found for &apos;{0}&apos;..
        /// </summary>
        internal static string Error_NoLogFound {
            get {
                return ResourceManager.GetString("Error_NoLogFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command &apos;{0}&apos; aborted due to no output and CPU activity for {1:0} seconds.  You may increase {2} setting to solve the issue..
        /// </summary>
        internal static string Error_ProcessAborted {
            get {
                return ResourceManager.GetString("Error_ProcessAborted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified project &apos;{0}&apos; is not valid. It needs to point to either a csproj/vbproj file or to a directory..
        /// </summary>
        internal static string Error_ProjectDoesNotExist {
            get {
                return ResourceManager.GetString("Error_ProjectDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a deployable project..
        /// </summary>
        internal static string Error_ProjectNotDeployable {
            get {
                return ResourceManager.GetString("Error_ProjectNotDeployable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to delete &apos;{0}&apos;. The deployment is currently active..
        /// </summary>
        internal static string Error_UnableToDeleteDeploymentActive {
            get {
                return ResourceManager.GetString("Error_UnableToDeleteDeploymentActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to delete &apos;{0}&apos;. No deployment found..
        /// </summary>
        internal static string Error_UnableToDeleteNoDeploymentFound {
            get {
                return ResourceManager.GetString("Error_UnableToDeleteNoDeploymentFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported status &apos;{0}&apos;..
        /// </summary>
        internal static string Error_UnsupportedStatus {
            get {
                return ResourceManager.GetString("Error_UnsupportedStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cleaning {0} repository..
        /// </summary>
        internal static string Log_CleaningRepository {
            get {
                return ResourceManager.GetString("Log_CleaningRepository", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using the following command to generate deployment script: &apos;azure site deploymentscript {0}&apos;..
        /// </summary>
        internal static string Log_DeploymentScriptGeneratorCommand {
            get {
                return ResourceManager.GetString("Log_DeploymentScriptGeneratorCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using cached version of deployment script (command: &apos;azure {0}&apos;)..
        /// </summary>
        internal static string Log_DeploymentScriptGeneratorUsingCache {
            get {
                return ResourceManager.GetString("Log_DeploymentScriptGeneratorUsingCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deployment successful..
        /// </summary>
        internal static string Log_DeploymentSuccessful {
            get {
                return ResourceManager.GetString("Log_DeploymentSuccessful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generating deployment script..
        /// </summary>
        internal static string Log_GeneratingDeploymentScript {
            get {
                return ResourceManager.GetString("Log_GeneratingDeploymentScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found solution &apos;{0}&apos; with no deployable projects. Deploying files instead..
        /// </summary>
        internal static string Log_NoDeployableProjects {
            get {
                return ResourceManager.GetString("Log_NoDeployableProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preparing deployment for commit id &apos;{0}&apos;..
        /// </summary>
        internal static string Log_PreparingDeployment {
            get {
                return ResourceManager.GetString("Log_PreparingDeployment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unknown error has occurred. Check the diagnostic log for details..
        /// </summary>
        internal static string Log_UnexpectedError {
            get {
                return ResourceManager.GetString("Log_UnexpectedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating branch &apos;{0}&apos;..
        /// </summary>
        internal static string Log_UpdatingBranch {
            get {
                return ResourceManager.GetString("Log_UpdatingBranch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating submodules..
        /// </summary>
        internal static string Log_UpdatingSubmodules {
            get {
                return ResourceManager.GetString("Log_UpdatingSubmodules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Building and Deploying &apos;{0}&apos;..
        /// </summary>
        internal static string Status_BuildingAndDeploying {
            get {
                return ResourceManager.GetString("Status_BuildingAndDeploying", resourceCulture);
            }
        }
    }
}
