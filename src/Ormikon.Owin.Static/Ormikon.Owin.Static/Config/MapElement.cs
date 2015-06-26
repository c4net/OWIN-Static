using System;
using System.ComponentModel;
using System.Configuration;

namespace Ormikon.Owin.Static.Config
{
    /// <summary>
    /// OWIN Static configuration element
    /// <example>
    /// <map path="/scripts" sources="..\..\Scripts" cached="false" expires="2020-01-01" include="*.js" exclude="**\*1.6.4.js" />
    /// </example>
    /// </summary>
    public class MapElement
    {

        public MapElement()
        {
            MaxAge = "0";
            DefaultFile = StaticSettings.DefaultFileValue;
            RedirectIfFolder = true;
        }

        public string Path { get; set; }

        public string Sources { get; set; }

        public bool Cached { get; set; }

        public DateTimeOffset Expires { get; set; }

        [DefaultValue("0")]
        public string MaxAge { get; set; }

        [DefaultValue(StaticSettings.DefaultFileValue)]
        public string DefaultFile { get; set; }

        [DefaultValue(true)]
        public bool RedirectIfFolder { get; set; }

        public string Include { get; set; }

        public string Exclude { get; set; }

        public bool AllowHidden { get; set; }
    }
}
