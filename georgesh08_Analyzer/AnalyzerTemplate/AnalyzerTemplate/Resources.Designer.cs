//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnalyzerTemplate {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AnalyzerTemplate.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Found Convert expression, Parse should be used instead.
        /// </summary>
        internal static string CasterAnalyzerDescription {
            get {
                return ResourceManager.GetString("CasterAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expression contains Convert statement.
        /// </summary>
        internal static string CasterAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("CasterAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Convert expression was found.
        /// </summary>
        internal static string CasterAnalyzerTitle {
            get {
                return ResourceManager.GetString("CasterAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use local variable instead private field.
        /// </summary>
        internal static string PrivateFieldAnalyzerDescription {
            get {
                return ResourceManager.GetString("PrivateFieldAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove redundant private field .
        /// </summary>
        internal static string PrivateFieldAnalyzerTitle {
            get {
                return ResourceManager.GetString("PrivateFieldAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Private field can be deleted.
        /// </summary>
        internal static string PrivateFieldMessageFormat {
            get {
                return ResourceManager.GetString("PrivateFieldMessageFormat", resourceCulture);
            }
        }
    }
}
