namespace Logistica.Domain.Resources {
    using System;
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class DomainMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DomainMessages() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Logistica.Domain.Resources.DomainMessages", typeof(DomainMessages).Assembly);
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
        
        public static string Validation_MissingIdentity {
            get {
                return ResourceManager.GetString("Validation_MissingIdentity", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Validation_InvalidDestination {
            get {
                return ResourceManager.GetString("Validation_InvalidDestination", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Validation_InvalidWeight {
            get {
                return ResourceManager.GetString("Validation_InvalidWeight", resourceCulture) ?? string.Empty;
            }
        }
        
        public static string Validation_InvalidDate {
            get {
                return ResourceManager.GetString("Validation_InvalidDate", resourceCulture) ?? string.Empty;
            }
        }
    }
}
