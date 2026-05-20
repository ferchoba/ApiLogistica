namespace Logistica.API.Resources {
    using System;
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ApiMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApiMessages() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Logistica.API.Resources.ApiMessages", typeof(ApiMessages).Assembly);
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
        
        public static string Http_EmptyFile {
            get {
                return ResourceManager.GetString("Http_EmptyFile", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Http_UnsupportedFormat {
            get {
                return ResourceManager.GetString("Http_UnsupportedFormat", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Http_UnhandledException {
            get {
                return ResourceManager.GetString("Http_UnhandledException", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Http_InternalServerError {
            get {
                return ResourceManager.GetString("Http_InternalServerError", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string FormatGuard_EmptyFile {
            get {
                return ResourceManager.GetString("FormatGuard_EmptyFile", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string FormatGuard_UnsupportedFormat {
            get {
                return ResourceManager.GetString("FormatGuard_UnsupportedFormat", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string FormatGuard_ExtensionMismatch {
            get {
                return ResourceManager.GetString("FormatGuard_ExtensionMismatch", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string FormatGuard_MimeTypeMismatch {
            get {
                return ResourceManager.GetString("FormatGuard_MimeTypeMismatch", resourceCulture) ?? string.Empty;
            }
        }
    }
}
