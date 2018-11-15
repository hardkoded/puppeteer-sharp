using System;
using System.Linq;
using System.Collections.Generic;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.PageAccessibility
{
    internal class AXNode
    {
        internal AccessibilityGetFullAXTreeResponse Payload { get; }
        public List<AXNode> Children { get; }

        private readonly string _name;
        private string _role;
        private bool _richlyEditable;
        private bool _editable;
        private bool _focusable;
        private bool _expanded;
        private bool? _cachedHasFocusableChild;

        public AXNode(AccessibilityGetFullAXTreeResponse payload)
        {
            Payload = payload;
            Children = new List<AXNode>();

            _name = payload.Name ?? string.Empty;
            _role = payload.Role ?? "Unknown";

            foreach (var property in payload.Properties)
            {
                if (property.Name == "editable")
                {
                    _richlyEditable = property.Value.Value == "richtext";
                    _editable = true;
                }
                if (property.Name == "focusable")
                {
                    _focusable = property.Value.Value == "true";
                }
                if (property.Name == "expanded")
                {
                    _expanded = property.Value.Value == "true";
                }
            }
        }

        internal static AXNode CreateTree(AccessibilityGetFullAXTreeResponse[] payloads)
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

        private bool IsPlainTextField()
        {
            if (_richlyEditable)
            {
                return false;
            }
            if (_editable)
            {
                return true;
            }
            return _role == "textbox" || _role == "ComboBox" || _role == "searchbox";
        }

        private bool IsTextOnlyObject()
            => _role == "LineBreak" ||
                _role == "text" ||
                _role == "InlineTextBox";

        private bool HasFocusableChild()
        {
            if (!_cachedHasFocusableChild.HasValue)
            {
                _cachedHasFocusableChild = false;
                foreach (var child in Children)
                {
                    if (child._focusable || child.HasFocusableChild())
                    {
                        _cachedHasFocusableChild = true;
                        break;
                    }
                }
            }
            return _cachedHasFocusableChild.Value;
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
            if (_focusable && !string.IsNullOrEmpty(_name))
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
            if (_role == "Ignored")
            {
                return false;
            }

            if (_focusable || _richlyEditable)
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
            throw new NotImplementedException();
        }
    }
}