using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// One node of the solved Typology tree as rendered by the area view: the bucket name, description, color and children.
    /// <para>The <see cref="Color"/> is a CSS hex string (<c>#rrggbb</c>) read from the node's <c>TypologyAppearance</c> via <see cref="Query.Color(DiGi.Typology.Visual.Classes.TypologyAppearance?)"/>, not a <c>System.Drawing.Color</c> — the view paints CSS, and a serialized color type would add a dependency the page does not need.</para>
    /// <para>The <see cref="Path"/> is the filing index chain of this node, one integer per level from the root. The view uses it to locate the node in the tree for the centroid join with the building dot positions.</para>
    /// </summary>
    public class TypologyTreeNodeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyTreeNodeViewModel"/> class.
        /// </summary>
        public TypologyTreeNodeViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyTreeNodeViewModel"/> class.
        /// </summary>
        /// <param name="name">The bucket name, as the solver named it.</param>
        /// <param name="description">The bucket description, or null when the column carries none.</param>
        /// <param name="color">The bucket color as a CSS hex string, or null when the rule maps none for this bucket.</param>
        /// <param name="path">The typology path, one filing index per level from the root.</param>
        /// <param name="children">The sub-typology nodes, or null when this node is a leaf.</param>
        public TypologyTreeNodeViewModel(string? name, string? description, string? color, List<int> path, List<TypologyTreeNodeViewModel>? children)
        {
            Name = name;
            Description = description;
            Color = color;
            Path = path;
            Children = children;
        }

        /// <summary>
        /// Gets the bucket name, as the solver named it: the level's column name and the rule data's text.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets the bucket description, the column's description when the column carries one.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the bucket color as a CSS hex string (<c>#rrggbb</c>), or null when the rule maps no appearance for this bucket.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets the typology path, one filing index per level from the root.
        /// </summary>
        public List<int> Path { get; set; } = [];

        /// <summary>
        /// Gets the sub-typology nodes, or null when this node is a leaf.
        /// </summary>
        public List<TypologyTreeNodeViewModel>? Children { get; set; }
    }
}
