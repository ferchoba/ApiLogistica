namespace Logistica.Application.Resources {
    using System;
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppMessages() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Logistica.Application.Resources.AppMessages", typeof(AppMessages).Assembly);
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
        
        public static string Log_OrchestrationStarted {
            get {
                return ResourceManager.GetString("Log_OrchestrationStarted", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Log_OrchestrationEnded {
            get {
                return ResourceManager.GetString("Log_OrchestrationEnded", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Log_DelegatingPersistence {
            get {
                return ResourceManager.GetString("Log_DelegatingPersistence", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Validation_DuplicateOrder {
            get {
                return ResourceManager.GetString("Validation_DuplicateOrder", resourceCulture) ?? string.Empty;
            }
        }
    }
}
