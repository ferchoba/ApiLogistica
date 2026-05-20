namespace Logistica.Infrastructure.Resources {
    using System;
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class InfraMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal InfraMessages() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Logistica.Infrastructure.Resources.InfraMessages", typeof(InfraMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string Parser_StartRead {
            get {
                return ResourceManager.GetString("Parser_StartRead", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_InsufficientColumns {
            get {
                return ResourceManager.GetString("Parser_InsufficientColumns", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_ErrorParsingLine {
            get {
                return ResourceManager.GetString("Parser_ErrorParsingLine", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_ConversionError {
            get {
                return ResourceManager.GetString("Parser_ConversionError", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_EndRead {
            get {
                return ResourceManager.GetString("Parser_EndRead", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_InvalidJsonStructure {
            get {
                return ResourceManager.GetString("Parser_InvalidJsonStructure", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_NullJsonRecord {
            get {
                return ResourceManager.GetString("Parser_NullJsonRecord", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_XmlProcessError {
            get {
                return ResourceManager.GetString("Parser_XmlProcessError", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Parser_MissingRequiredXmlNode {
            get {
                return ResourceManager.GetString("Parser_MissingRequiredXmlNode", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Repo_MissingConnectionString {
            get {
                return ResourceManager.GetString("Repo_MissingConnectionString", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Repo_BulkInsertStart {
            get {
                return ResourceManager.GetString("Repo_BulkInsertStart", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Repo_BulkInsertSuccess {
            get {
                return ResourceManager.GetString("Repo_BulkInsertSuccess", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Repo_BulkInsertFailed {
            get {
                return ResourceManager.GetString("Repo_BulkInsertFailed", resourceCulture) ?? string.Empty;
            }
        }
    }
}
