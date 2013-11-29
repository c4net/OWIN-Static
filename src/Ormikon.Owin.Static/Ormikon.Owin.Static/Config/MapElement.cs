﻿using System;
using System.Configuration;

namespace Ormikon.Owin.Static.Config
{
    /// <summary>
    /// OWIN Static configuration element
    /// <example>
    /// <map path="/scripts" sources="..\..\Scripts" cached="false" expires="2020-01-01" include="*.js" exclude="**\*1.6.4.js" />
    /// </example>
    /// </summary>
    public class MapElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = false)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("sources", IsRequired = true)]
        public string Sources
        {
            get
            {
                return (string)this["sources"];
            }
            set
            {
                this["sources"] = value;
            }
        }

        [ConfigurationProperty("cached", IsRequired = false, DefaultValue = false)]
        public bool Cached
        {
            get
            {
                return (bool)this["cached"];
            }
            set
            {
                this["cached"] = value;
            }
        }

        [ConfigurationProperty("expires", IsRequired = false)]
        public DateTimeOffset Expires
        {
            get
            {
                object result = this["expires"];
                return result == null ? DateTimeOffset.MinValue : (DateTimeOffset)result;
            }
            set
            {
                this["expires"] = value;
            }
        }

        [ConfigurationProperty("maxAge", IsRequired = false, DefaultValue = "0")]
        public string MaxAge
        {
            get { return (string) this["maxAge"]; }
            set { this["maxAge"] = value; }
        }

        [ConfigurationProperty("default", IsRequired = false, DefaultValue = StaticSettings.DefaultFileValue)]
        public string DefaultFile
        {
            get { return (string)this["default"]; }
            set { this["default"] = value; }
        }

        [ConfigurationProperty("redirectIfFolder", IsRequired = false, DefaultValue = true)]
        public bool RedirectIfFolder
        {
            get { return (bool)this["redirectIfFolder"]; }
            set { this["redirectIfFolder"] = value; }
        }

        [ConfigurationProperty("include", IsRequired = false)]
        public string Include
        {
            get
            {
                return (string)this["include"];
            }
            set
            {
                this["include"] = value;
            }
        }

        [ConfigurationProperty("exclude", IsRequired = false)]
        public string Exclude
        {
            get
            {
                return (string)this["exclude"];
            }
            set
            {
                this["exclude"] = value;
            }
        }

        [ConfigurationProperty("hidden", IsRequired = false, DefaultValue = false)]
        public bool AllowHidden
        {
            get
            {
                return (bool)this["hidden"];
            }
            set
            {
                this["hidden"] = value;
            }
        }
    }
}
