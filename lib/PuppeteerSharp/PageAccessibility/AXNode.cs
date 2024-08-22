using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.PageAccessibility
{
    internal class AXNode
    {
        private readonly string _name;
        private readonly bool _richlyEditable;
        private readonly bool _editable;
        private readonly bool _hidden;
        private readonly string _role;
        private readonly bool _ignored;
        private bool? _cachedHasFocusableChild;

        private AXNode(AccessibilityGetFullAXTreeResponse.AXTreeNode payload)
        {
            Payload = payload;

            _name = payload.Name != null ? payload.Name.Value.ToObject<string>() : string.Empty;
            _role = payload.Role != null ? payload.Role.Value.ToObject<string>() : "Unknown";
            _ignored = payload.Ignored;

            _richlyEditable = payload.Properties?.FirstOrDefault(p => p.Name == "editable")?.Value.Value.ToObject<string>() == "richtext";
            _editable |= _richlyEditable;
            _hidden = payload.Properties?.FirstOrDefault(p => p.Name == "hidden")?.Value.Value.ToObject<bool>() == true;
            Focusable = payload.Properties?.FirstOrDefault(p => p.Name == "focusable")?.Value.Value.ToObject<bool>() == true;
        }

        public List<AXNode> Children { get; } = new();

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
                case "image":
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
                case "treeitem":
                    return true;
                default:
                    return false;
            }
        }

        internal bool IsInteresting(bool insideControl)
        {
            if (_role == "Ignored" || _hidden || _ignored)
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
            var properties = new Dictionary<string, JsonElement?>();

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
                Name = properties.GetValue("name")?.ToObject<string>(),
                Value = properties.GetValue("value")?.ToObject<object>()?.ToString(),
                Description = properties.GetValue("description")?.ToObject<string>(),
                KeyShortcuts = properties.GetValue("keyshortcuts")?.ToObject<string>(),
                RoleDescription = properties.GetValue("roledescription")?.ToObject<string>(),
                ValueText = properties.GetValue("valuetext")?.ToObject<string>(),
                Disabled = properties.GetValue("disabled")?.ToObject<bool>() ?? false,
                Expanded = properties.GetValue("expanded")?.ToObject<bool>() ?? false,

                // RootWebArea's treat focus differently than other nodes. They report whether their frame  has focus,
                // not whether focus is specifically on the root node.
                Focused = properties.GetValue("focused")?.ToObject<bool>() == true && _role != "RootWebArea",
                Modal = properties.GetValue("modal")?.ToObject<bool>() ?? false,
                Multiline = properties.GetValue("multiline")?.ToObject<bool>() ?? false,
                Multiselectable = properties.GetValue("multiselectable")?.ToObject<bool>() ?? false,
                Readonly = properties.GetValue("readonly")?.ToObject<bool>() ?? false,
                Required = properties.GetValue("required")?.ToObject<bool>() ?? false,
                Selected = properties.GetValue("selected")?.ToObject<bool>() ?? false,
                Checked = GetCheckedState(properties.GetValue("checked")?.ToObject<string>()),
                Pressed = GetCheckedState(properties.GetValue("pressed")?.ToObject<string>()),
                Level = properties.GetValue("level")?.ToObject<int>() ?? 0,
                ValueMax = properties.GetValue("valuemax")?.ToObject<int>() ?? 0,
                ValueMin = properties.GetValue("valuemin")?.ToObject<int>() ?? 0,
                AutoComplete = GetIfNotFalse(properties.GetValue("autocomplete")?.ToObject<string>()),
                HasPopup = GetIfNotFalse(properties.GetValue("haspopup")?.ToObject<string>()),
                Invalid = GetIfNotFalse(properties.GetValue("invalid")?.ToObject<string>()),
                Orientation = GetIfNotFalse(properties.GetValue("orientation")?.ToObject<string>()),
            };

            return node;
        }

        private bool IsPlainTextField()
            => !_richlyEditable && (_editable || _role == "textbox" || _role == "ComboBox" || _role == "searchbox");

        private bool IsTextOnlyObject()
            => _role == "LineBreak" ||
                _role == "text" ||
                _role == "InlineTextBox" ||
                _role == "StaticText";

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
