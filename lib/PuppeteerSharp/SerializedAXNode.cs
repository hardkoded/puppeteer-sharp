using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class SerializedAXNode : IEquatable<SerializedAXNode>
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string KeyShortcuts { get; set; }
        public string RoleDescription { get; set; }
        public string ValueText { get; set; }
        public bool Disabled { get; set; }
        public bool Expanded { get; set; }
        public bool Focused { get; set; }
        public bool Modal { get; set; }
        public bool Multiline { get; set; }
        public bool Multiselectable { get; set; }
        public bool Readonly { get; set; }
        public bool Required { get; set; }
        public bool Selected { get; set; }
        public bool Checked { get; set; }
        public bool Pressed { get; set; }
        public int Level { get; set; }
        public int ValueMin { get; set; }
        public int ValueMax { get; set; }
        public string Autocomplete { get; set; }
        public string Haspopup { get; set; }
        public string Invalid { get; set; }
        public string Orientation { get; set; }
        public SerializedAXNode[] Children { get; set; }

        public bool Equals(SerializedAXNode other)
            => ReferenceEquals(this, other) ||
                (
                    Role.Equals(other.Role, StringComparison.OrdinalIgnoreCase) &&
                    Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                    Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase) &&
                    Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase) &&
                    KeyShortcuts.Equals(other.KeyShortcuts, StringComparison.OrdinalIgnoreCase) &&
                    RoleDescription.Equals(other.RoleDescription, StringComparison.OrdinalIgnoreCase) &&
                    ValueText.Equals(other.ValueText, StringComparison.OrdinalIgnoreCase) &&
                    Autocomplete.Equals(other.Haspopup, StringComparison.OrdinalIgnoreCase) &&
                    Haspopup.Equals(other.Haspopup, StringComparison.OrdinalIgnoreCase) &&
                    Orientation.Equals(other.Orientation, StringComparison.OrdinalIgnoreCase) &&
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
                    Children.Equals(other.Children)
                );

        public override bool Equals(object obj) => obj is SerializedAXNode s && Equals(s);
        public override int GetHashCode()
            => Role.GetHashCode() ^
                Name.GetHashCode() ^
                Value.GetHashCode() ^
                Description.GetHashCode() ^
                KeyShortcuts.GetHashCode() ^
                RoleDescription.GetHashCode() ^
                ValueText.GetHashCode() ^
                Autocomplete.GetHashCode() ^
                Haspopup.GetHashCode() ^
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