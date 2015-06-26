using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ormikon.Owin.Static.Config
{
    /// <summary>
    /// OWIN Static configuration section
    /// <example>
    /// <configSections>
    ///   <section name="owinStatic" type="Ormikon.Owin.Static.Config.Section, Ormikon.Owin.Static" />
    /// </configSections>
    /// <owinStatic>
    ///   <maps>
    ///     <map path="/scripts" sources="..\..\Scripts" cached="false" expires="2020-01-01" include="*.js" exclude="**\*1.6.4.js" />
    ///     <map path="/styles" sources="..\..\Styles" cached="false" expires="2020-01-01" include="*.css" exclude="**\*debug.css" />
    ///     <map path="/home" sources="..\..\Index.html" />
    ///   </maps>
    /// </owinStatic>
    /// </example>
    /// </summary>
    public class Section
    {
        public const string Name = "owinStatic";
        private readonly static Section _defaultSection = new Section();
       
        /// <summary>
        /// Is any mapping found in the configuration
        /// </summary>
        /// <returns></returns>
        public bool HasMappings()
        {
            return Sources != null && Sources.Count > 0;
        }

        /// <summary>
        /// Wraps mappings from the configuration into Settings and enumerates them
        /// </summary>
        /// <returns>Settings collection</returns>
        public IEnumerable<Settings> EnumerateSettings()
        {
            if (!HasMappings())
                yield break;
            foreach (MapElement map in Sources)
            {
                yield return new Settings(map);
            }
        }

        /// <summary>
        /// Default instance of the section
        /// </summary>
        public static Section Default
        {
            get
            {
                return _defaultSection;
            }
        }

        public string MapPath { get; set; }

        public IList<MapElement> Sources = new List<MapElement>();
    }
}
