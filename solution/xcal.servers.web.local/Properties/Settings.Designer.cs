﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using reexjungle.xmisc.infrastructure.concretes.operations;

namespace reexjungle.xcal.application.server.web.local.Properties {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("xCal - iCalendar Web Services")]
        public string service_name {
            get {
                return ((string)(this["service_name"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("localhost:6379")]
        public string redis_server {
            get {
                return ((string)(this["redis_server"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Server = localhost; Uid = local; Pwd = local; allow user variables=true;")]
        public string mysql_server {
            get {
                return ((string)(this["mysql_server"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("xcal_local_db")]
        public string main_db_name {
            get {
                return ((string)(this["main_db_name"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("False")]
        public bool overwrite_db {
            get {
                return ((bool)(this["overwrite_db"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [SpecialSetting(SpecialSetting.ConnectionString)]
        [DefaultSettingValue("Server = localhost; Uid = local; Pwd = local; Database=xcal_elmah_local_db; allow" +
            " user variables=true;")]
        public string elmah_mysql_db {
            get {
                return ((string)(this["elmah_mysql_db"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("xcal_elmah_local_db")]
        public string elmah_db_name {
            get {
                return ((string)(this["elmah_db_name"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("elmah_error")]
        public string elmah_error_table {
            get {
                return ((string)(this["elmah_error_table"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("xcal_nlog_local_db")]
        public string nlog_db_name {
            get {
                return ((string)(this["nlog_db_name"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("rdbms")]
        public StorageType main_storage {
            get {
                return ((StorageType)(this["main_storage"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("nosql")]
        public StorageType auth_storage {
            get {
                return ((StorageType)(this["auth_storage"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("nosql")]
        public StorageType cache_storage {
            get {
                return ((StorageType)(this["cache_storage"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("localhost:11000")]
        public string memcached_server {
            get {
                return ((string)(this["memcached_server"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("reezure")]
        public string azure_server {
            get {
                return ((string)(this["azure_server"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("http://localhost:3105/")]
        public string local {
            get {
                return ((string)(this["local"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("False")]
        public bool recreate_auth_tables {
            get {
                return ((bool)(this["recreate_auth_tables"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("dToWrTe15djJBAXJyBWXsb6iQ")]
        public string oauth_twitter_ConsumerKey {
            get {
                return ((string)(this["oauth_twitter_ConsumerKey"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("aaeTQ4ynWvmnGF0U9kjQh7LS5svUad7YVpl6nVjSjWtWwqXdSD")]
        public string oauth_twitter_ConsumerSecret {
            get {
                return ((string)(this["oauth_twitter_ConsumerSecret"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("529819727159657")]
        public string oauth_facebook_AppId {
            get {
                return ((string)(this["oauth_facebook_AppId"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("9fababbeffa80add8a2f1ee608d2b09a")]
        public string oauth_facebook_AppSecret {
            get {
                return ((string)(this["oauth_facebook_AppSecret"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("1062386930271-eu7mdqe965b08tsf6c4790usl53dtu5l.apps.googleusercontent.com")]
        public string oauth_google_ConsumerKey {
            get {
                return ((string)(this["oauth_google_ConsumerKey"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("g-1mITMd85yQYPCI6Xl2eAS8")]
        public string oauth_google_ConsumerSecret {
            get {
                return ((string)(this["oauth_google_ConsumerSecret"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("xs")]
        public string oauth_linkedin_ConsumerKey {
            get {
                return ((string)(this["oauth_linkedin_ConsumerKey"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("77l4kofoki9x6o")]
        public string oauth_linkedin_ConsumerSecret {
            get {
                return ((string)(this["oauth_linkedin_ConsumerSecret"]));
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("nNPTFQhHo8aMxBa9")]
        public string Setting {
            get {
                return ((string)(this["Setting"]));
            }
        }
    }
}
