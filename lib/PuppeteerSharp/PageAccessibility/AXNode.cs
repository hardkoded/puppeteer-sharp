using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.PageAccessibility
{
    internal class AXNode
    {
        private readonly string _name;
        private readonly bool _richlyEditable;
        private readonly bool _editable;
        private readonly bool _hidden;
        private readonly string _role;
        private bool? _cachedHasFocusableChild;

        public AXNode(AccessibilityGetFullAXTreeResponse.AXTreeNode payload)
        {
            Payload = payload;
            Children = new List<AXNode>();

            _name = payload.Name != null ? payload.Name.Value.Deserialize<string>() : string.Empty;
            _role = payload.Role != null ? payload.Role.Value.Deserialize<string>() : "Unknown";

            _richlyEditable = payload.Properties?.FirstOrDefault(p => p.Name == "editable")?.Value.Value.Deserialize<string>() == "richtext";
            _editable |= _richlyEditable;
            _hidden = payload.Properties?.FirstOrDefault(p => p.Name == "hidden")?.Value.Value.Deserialize<bool>() == true;
            Focusable = payload.Properties?.FirstOrDefault(p => p.Name == "focusable")?.Value.Value.Deserialize<bool>() == true;
        }

        public List<AXNode> Children { get; }

        public bool Focusable { get; set; }

        internal AccessibilityGetFullAXTreeResponse.AXTreeNode Payload { get; }

        internal static AXNode CreateTree(IEnumerable<AccessibilityGetFullAXTreeResponse.AXTreeNode> payloads)
        {
            var nodeById = new Dictionary<string, AXNode>();
            foreach (var payload in payloads)
            {
                nodeById[payload.NodeId] = new AXNode(payload);
            }

            foreach (var node in nodeById.Values)
            {
                foreach (var childId in node.Payload.ChildIds)
                {
                    node.Children.Add(nodeById[childId]);
                }
            }

            return nodeById.Values.FirstOrDefault();
        }

        internal AXNode Find(Func<AXNode, bool> predicate)
        {
            if (predicate(this))
            {
                return this;
            }

            foreach (var child in Children)
            {
                var result = child.Find(predicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        internal bool IsLeafNode()
        {
            if (Children.Count == 0)
            {
                return true;
            }

            // These types of objects may have children that we use as internal
            // implementation details, but we want to expose them as leaves to platform
            // accessibility APIs because screen readers might be confused if they find
            // any children.
            if (IsPlainTextField() || IsTextOnlyObject())
            {
                return true;
            }

            // Roles whose children are only presentational according to the ARIA and
            // HTML5 Specs should be hidden from screen readers.
            // (Note that whilst ARIA buttons can have only presentational children, HTML5
            // buttons are allowed to have content.)
            switch (_role)
            {
                case "doc-cover":
                case "graphics-symbol":
                case "img":
                case "Meter":
                case "scrollbar":
                case "slider":
                case "separator":
                case "progressbar":
                    return true;
            }

            // Here and below: Android heuristics
            if (HasFocusableChild())
            {
                return false;
            }

            if (Focusable && !string.IsNullOrEmpty(_name))
            {
                return true;
            }

            if (_role == "heading" && !string.IsNullOrEmpty(_name))
            {
                return true;
            }

            return false;
        }

        internal bool IsControl()
        {
            switch (_role)
            {
                case "button":
                case "checkbox":
                case "ColorWell":
                case "combobox":
                case "DisclosureTriangle":
                case "listbox":
                case "menu":
                case "menubar":
                case "menuitem":
                case "menuitemcheckbox":
                case "menuitemradio":
                case "radio":
                case "scrollbar":
                case "searchbox":
                case "slider":
                case "spinbutton":
                case "switch":
                case "tab":
                case "textbox":
                case "tree":
                    return true;
                default:
                    return false;
            }
        }

        internal bool IsInteresting(bool insideControl)
        {
            if (_role == "Ignored" || _hidden)
            {
                return false;
            }

            if (Focusable || _richlyEditable)
            {
                return true;
            }

            // If it's not focusable but has a control role, then it's interesting.
            if (IsControl())
            {
                return true;
            }

            // A non focusable child of a control is not interesting
            if (insideControl)
            {
                return false;
            }

            return IsLeafNode() && !string.IsNullOrEmpty(_name);
        }

        internal SerializedAXNode Serialize()
        {
            var properties = new Dictionary<string, JsonElement>();

            if (Payload.Properties != null)
            {
                foreach (var property in Payload.Properties)
                {
                    properties[property.Name.ToLower(CultureInfo.CurrentCulture)] = property.Value.Value;
                }
            }

            if (Payload.Name != null)
            {
                properties["name"] = Payload.Name.Value;
            }

            if (Payload.Value != null)
            {
                properties["value"] = Payload.Value.Value;
            }

            if (Payload.Description != null)
            {
                properties["description"] = Payload.Description.Value;
            }

            var node = new SerializedAXNode
            {
                Role = _role,
                Name = properties.GetValueOrDefault("name").Deserialize<string>(),
                Value = properties.GetValueOrDefault("value").Deserialize<string>(),
                Description = properties.GetValueOrDefault("description").Deserialize<string>(),
                KeyShortcuts = properties.GetValueOrDefault("keyshortcuts").Deserialize<string>(),
                RoleDescription = properties.GetValueOrDefault("roledescription").Deserialize<string>(),
                ValueText = properties.GetValueOrDefault("valuetext").Deserialize<string>(),
                Disabled = properties.GetValueOrDefault("disabled").Deserialize<bool?>() ?? false,
                Expanded = properties.GetValueOrDefault("expanded").Deserialize<bool?>() ?? false,

                // RootWebArea's treat focus differently than other nodes. They report whether their frame  has focus,
                // not whether focus is specifically on the root node.
                Focused = properties.GetValueOrDefault("focused").Deserialize<bool>() == true && _role != "RootWebArea",
                Modal = properties.GetValueOrDefault("modal").Deserialize<bool?>() ?? false,
                Multiline = properties.GetValueOrDefault("multiline").Deserialize<bool?>() ?? false,
                Multiselectable = properties.GetValueOrDefault("multiselectable").Deserialize<bool?>() ?? false,
                Readonly = properties.GetValueOrDefault("readonly").Deserialize<bool?>() ?? false,
                Required = properties.GetValueOrDefault("required").Deserialize<bool?>() ?? false,
                Selected = properties.GetValueOrDefault("selected").Deserialize<bool?>() ?? false,
                Checked = GetCheckedState(properties.GetValueOrDefault("checked").Deserialize<string>()),
                Pressed = GetCheckedState(properties.GetValueOrDefault("pressed").Deserialize<string>()),
                Level = properties.GetValueOrDefault("level").Deserialize<int?>() ?? 0,
                ValueMax = properties.GetValueOrDefault("valuemax").Deserialize<int?>() ?? 0,
                ValueMin = properties.GetValueOrDefault("valuemin").Deserialize<int?>() ?? 0,
                AutoComplete = GetIfNotFalse(properties.GetValueOrDefault("autocomplete").Deserialize<string>()),
                HasPopup = GetIfNotFalse(properties.GetValueOrDefault("haspopup").Deserialize<string>()),
                Invalid = GetIfNotFalse(properties.GetValueOrDefault("invalid").Deserialize<string>()),
                Orientation = GetIfNotFalse(properties.GetValueOrDefault("orientation").Deserialize<string>()),
            };

            return node;
        }

        private bool IsPlainTextField()
            => !_richlyEditable && (_editable || _role == "textbox" || _role == "ComboBox" || _role == "searchbox");

        private bool IsTextOnlyObject()
            => _role == "LineBreak" ||
                _role == "text" ||
                _role == "InlineTextBox";

        private bool HasFocusableChild()
        {
            return _cachedHasFocusableChild ??= Children.Any(c => c.Focusable || c.HasFocusableChild());
        }

        private string GetIfNotFalse(string value) => value != null && value != "false" ? value : null;

        private CheckedState GetCheckedState(string value)
        {
            switch (value)
            {
                case "mixed":
                    return CheckedState.Mixed;
                case "true":
                    return CheckedState.True;
                default:
                    return CheckedState.False;
            }
        }
    }
}
