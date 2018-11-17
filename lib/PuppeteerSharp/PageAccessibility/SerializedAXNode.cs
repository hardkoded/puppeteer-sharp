using System;
using System.Linq;

namespace PuppeteerSharp.PageAccessibility
{
    /// <summary>
    /// AXNode.
    /// </summary>
    public class SerializedAXNode : IEquatable<SerializedAXNode>
    {
        /// <summary>
        /// The <see fref="https://www.w3.org/TR/wai-aria/#usage_intro">role</see>.
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// A human readable name for the node.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The current value of the node.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// An additional human readable description of the node.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Keyboard shortcuts associated with this node.
        /// </summary>
        public string KeyShortcuts { get; set; }
        /// <summary>
        /// A human readable alternative to the role.
        /// </summary>
        public string RoleDescription { get; set; }
        /// <summary>
        /// A description of the current value.
        /// </summary>
        public string ValueText { get; set; }
        /// <summary>
        /// Whether the node is disabled.
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// Whether the node is expanded or collapsed.
        /// </summary>
        public bool Expanded { get; set; }
        /// <summary>
        /// Whether the node is focused.
        /// </summary>
        public bool Focused { get; set; }
        /// <summary>
        /// Whether the node is <see href="https://en.wikipedia.org/wiki/Modal_window">modal</see>.
        /// </summary>
        public bool Modal { get; set; }
        /// <summary>
        /// Whether the node text input supports multiline.
        /// </summary>
        public bool Multiline { get; set; }
        /// <summary>
        /// Whether more than one child can be selected.
        /// </summary>
        public bool Multiselectable { get; set; }
        /// <summary>
        /// Whether the node is read only.
        /// </summary>
        public bool Readonly { get; set; }
        /// <summary>
        /// Whether the node is required.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Whether the node is selected in its parent node.
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Whether the checkbox is checked, or "mixed".
        /// </summary>
        public CheckedState Checked { get; set; }
        /// <summary>
        /// Whether the toggle button is checked, or "mixed".
        /// </summary>
        public CheckedState Pressed { get; set; }
        /// <summary>
        /// The level of a heading.
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// The minimum value in a node.
        /// </summary>
        public int ValueMin { get; set; }
        /// <summary>
        /// The maximum value in a node.
        /// </summary>
        public int ValueMax { get; set; }
        /// <summary>
        /// What kind of autocomplete is supported by a control.
        /// </summary>
        public string AutoComplete { get; set; }
        /// <summary>
        /// What kind of popup is currently being shown for a node.
        /// </summary>
        public string HasPopup { get; set; }
        /// <summary>
        /// Whether and in what way this node's value is invalid.
        /// </summary>
        public string Invalid { get; set; }
        /// <summary>
        /// Whether the node is oriented horizontally or vertically.
        /// </summary>
        public string Orientation { get; set; }
        /// <summary>
        /// Child nodes of this node, if any.
        /// </summary>
        public SerializedAXNode[] Children { get; set; }

        /// <inheritdoc/>
        public bool Equals(SerializedAXNode other)
            => ReferenceEquals(this, other) ||
                (
                    Role == other.Role &&
                    Name == other.Name &&
                    Value == other.Value &&
                    Description == other.Description &&
                    KeyShortcuts == other.KeyShortcuts &&
                    RoleDescription == other.RoleDescription &&
                    ValueText == other.ValueText &&
                    AutoComplete == other.AutoComplete &&
                    HasPopup == other.HasPopup &&
                    Orientation == other.Orientation &&
                    Disabled == other.Disabled &&
                    Expanded == other.Expanded &&
                    Focused == other.Focused &&
                    Modal == other.Modal &&
                    Multiline == other.Multiline &&
                    Multiselectable == other.Multiselectable &&
                    Readonly == other.Readonly &&
                    Required == other.Required &&
                    Selected == other.Selected &&
                    Checked == other.Checked &&
                    Pressed == other.Pressed &&
                    Level == other.Level &&
                    ValueMin == other.ValueMin &&
                    ValueMax == other.ValueMax &&
                    (Children == other.Children || Children.SequenceEqual(other.Children))
                );

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is SerializedAXNode s && Equals(s);
        /// <inheritdoc/>
        public override int GetHashCode()
            => Role.GetHashCode() ^
                Name.GetHashCode() ^
                Value.GetHashCode() ^
                Description.GetHashCode() ^
                KeyShortcuts.GetHashCode() ^
                RoleDescription.GetHashCode() ^
                ValueText.GetHashCode() ^
                AutoComplete.GetHashCode() ^
                HasPopup.GetHashCode() ^
                Orientation.GetHashCode() ^
                Disabled.GetHashCode() ^
                Expanded.GetHashCode() ^
                Focused.GetHashCode() ^
                Modal.GetHashCode() ^
                Multiline.GetHashCode() ^
                Multiselectable.GetHashCode() ^
                Readonly.GetHashCode() ^
                Required.GetHashCode() ^
                Selected.GetHashCode() ^
                Pressed.GetHashCode() ^
                Checked.GetHashCode() ^
                Level.GetHashCode() ^
                ValueMin.GetHashCode() ^
                ValueMax.GetHashCode() ^
                Children.GetHashCode();
    }
}