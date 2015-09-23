﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace reexjungle.xcal.application.server.web.dev2.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("reexjungle.xcal.application.server.web.dev2.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to CREATE PROCEDURE elmah_CreateLogTable()
        ///BEGIN
        ///	CREATE TABLE IF NOT EXISTS `elmah_error`(
        ///	  `ErrorId` CHAR(36) NOT NULL ,
        ///	  `Application` VARCHAR(60) NOT NULL ,
        ///	  `Host` VARCHAR(50) NOT NULL ,
        ///	  `Type` VARCHAR(100) NOT NULL ,
        ///	  `Source` VARCHAR(60) NOT NULL ,
        ///	  `Message` VARCHAR(500) NOT NULL ,
        ///	  `User` VARCHAR(50) NOT NULL ,
        ///	  `StatusCode` INT(10) NOT NULL ,
        ///	  `TimeUtc` DATETIME NOT NULL ,
        ///	  `Sequence` INT(10) NOT NULL AUTO_INCREMENT ,
        ///	  `AllXml` TEXT NOT NULL ,
        ///	  PRIMARY KEY (`Seq [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string elmah_mysql_CreateLogTable {
            get {
                return ResourceManager.GetString("elmah_mysql_CreateLogTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE elmah_GetErrorsXml (
        ///  IN App VARCHAR(60),
        ///  IN PageIndex INT(10),
        ///  IN PageSize INT(10),
        ///  OUT TotalCount INT(10)
        ///)
        ///NOT DETERMINISTIC
        ///READS SQL DATA
        ///BEGIN
        ///    
        ///    SELECT  count(*) INTO TotalCount
        ///    FROM    `elmah_error`
        ///    WHERE   `Application` = App;
        ///    SET @index = PageIndex * PageSize;
        ///    SET @count = PageSize;
        ///    SET @app = App;
        ///    PREPARE STMT FROM &apos;
        ///    SELECT
        ///        `ErrorId`,
        ///        `Application`,
        ///        `Host`,
        ///        `Type`,
        ///        `Source`,
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string elmah_mysql_GetErrorsXml {
            get {
                return ResourceManager.GetString("elmah_mysql_GetErrorsXml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE elmah_GetErrorXml (
        ///  IN Id CHAR(36),
        ///  IN App VARCHAR(60)
        ///)
        ///
        ///NOT DETERMINISTIC
        ///READS SQL DATA
        ///BEGIN
        ///    SELECT  `AllXml`
        ///    FROM    `elmah_error`
        ///    WHERE   `ErrorId` = Id AND `Application` = App;
        ///END;.
        /// </summary>
        internal static string elmah_mysql_GetErrorXml {
            get {
                return ResourceManager.GetString("elmah_mysql_GetErrorXml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE elmah_LogError (
        ///    IN ErrorId CHAR(36), 
        ///    IN Application varchar(60), 
        ///    IN Host VARCHAR(30), 
        ///    IN Type VARCHAR(100), 
        ///    IN Source VARCHAR(60), 
        ///    IN Message VARCHAR(500), 
        ///    IN User VARCHAR(50), 
        ///    IN AllXml TEXT, 
        ///    IN StatusCode INT(10), 
        ///    IN TimeUtc DATETIME
        ///)
        ///NOT DETERMINISTIC
        ///MODIFIES SQL DATA
        ///BEGIN
        ///    INSERT INTO `elmah_error` (
        ///        `ErrorId`, 
        ///        `Application`, 
        ///        `Host`, 
        ///        `Type`, 
        ///        `Source`, 
        ///        `Messag [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string elmah_mysql_LogError {
            get {
                return ResourceManager.GetString("elmah_mysql_LogError", resourceCulture);
            }
        }
    }
}
