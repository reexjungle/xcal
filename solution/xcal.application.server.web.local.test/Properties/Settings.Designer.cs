﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace reexjungle.xcal.application.server.web.dev.test.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://reexux.com/xcal/dev2/")]
        public string remote_server_redis {
            get {
                return ((string)(this["remote_server_redis"]));
            }
            set {
                this["remote_server_redis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("iCalendar Web Services Provider")]
        public string fpiDescription {
            get {
                return ((string)(this["fpiDescription"]));
            }
            set {
                this["fpiDescription"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("reexmonkey")]
        public string fpiOwner {
            get {
                return ((string)(this["fpiOwner"]));
            }
            set {
                this["fpiOwner"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EN")]
        public string fpiLanguageId {
            get {
                return ((string)(this["fpiLanguageId"]));
            }
            set {
                this["fpiLanguageId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public global::reexjungle.infrastructure.operations.contracts.Authority fpiAuthority
        {
            get {
                return ((global::reexjungle.infrastructure.operations.contracts.Authority)(this["fpiAuthority"]));
            }
            set {
                this["fpiAuthority"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:3105")]
        public string local_server {
            get {
                return ((string)(this["local_server"]));
            }
            set {
                this["local_server"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://reexux.com/xcal/dev1/")]
        public string remote_server_mysql {
            get {
                return ((string)(this["remote_server_mysql"]));
            }
            set {
                this["remote_server_mysql"] = value;
            }
        }
    }
}
