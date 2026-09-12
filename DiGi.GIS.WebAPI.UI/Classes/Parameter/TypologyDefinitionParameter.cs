using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The page state of the Typology definition page: the chain of selected building-data columns, each with its rule type, ranges and colors.
    /// <para>This is the wire contract of <c>POST /typology/definition/export</c> (request) and <c>POST /typology/definition/validate</c> (response), serialized camelCase by the application's default JSON options, exactly as <c>typology.js</c> holds it. The DiGi document (<see cref="DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter"/>) is composed and parsed on the server only, so the page never spells a <c>_type</c> or a <c>TypologyAppearanceCollection</c> key.</para>
    /// </summary>
    public class TypologyDefinitionParameter
    {
        /// <summary>
        /// Gets or sets the levels of the chain, root first.
        /// </summary>
        public List<TypologyDefinitionLevelParameter>? Levels { get; set; }
    }
}
