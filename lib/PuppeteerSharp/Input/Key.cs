﻿namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Utility class to be used with <see cref="Keyboard"/> operations
    /// </summary>
    public class Key
    {
        private readonly string _value;
        private Key(string value) => _value = value;

        public static readonly Key Cancel = new Key("Cancel");
        public static readonly Key Help = new Key("Help");
        public static readonly Key Backspace = new Key("Backspace");
        public static readonly Key Tab = new Key("Tab");
        public static readonly Key Clear = new Key("Clear");
        public static readonly Key Enter = new Key("Enter");
        public static readonly Key Shift = new Key("Shift");
        public static readonly Key Control = new Key("Control");
        public static readonly Key Alt = new Key("Alt");
        public static readonly Key Pause = new Key("Pause");
        public static readonly Key CapsLock = new Key("CapsLock");
        public static readonly Key Escape = new Key("Escape");
        public static readonly Key Convert = new Key("Convert");
        public static readonly Key NonConvert = new Key("NonConvert");
        public static readonly Key Accept = new Key("Accept");
        public static readonly Key ModeChange = new Key("ModeChange");
        public static readonly Key PageUp = new Key("PageUp");
        public static readonly Key PageDown = new Key("PageDown");
        public static readonly Key End = new Key("End");
        public static readonly Key Home = new Key("Home");
        public static readonly Key ArrowLeft = new Key("ArrowLeft");
        public static readonly Key ArrowUp = new Key("ArrowUp");
        public static readonly Key ArrowRight = new Key("ArrowRight");
        public static readonly Key ArrowDown = new Key("ArrowDown");
        public static readonly Key Select = new Key("Select");
        public static readonly Key Print = new Key("Print");
        public static readonly Key Execute = new Key("Execute");
        public static readonly Key PrintScreen = new Key("PrintScreen");
        public static readonly Key Insert = new Key("Insert");
        public static readonly Key Delete = new Key("Delete");
        public static readonly Key CloseParentheses = new Key(")");
        public static readonly Key ExclamationMark = new Key("!");
        public static readonly Key AtSign = new Key("@");
        public static readonly Key NumberSign = new Key("#");
        public static readonly Key DollarSign = new Key("$");
        public static readonly Key Percent = new Key("%");
        public static readonly Key Caret = new Key("^");
        public static readonly Key Ampersand = new Key("&");
        public static readonly Key Asterisk = new Key("*");
        public static readonly Key OpenParentheses = new Key("(");
        public static readonly Key Meta = new Key("Meta");
        public static readonly Key ContextMenu = new Key("ContextMenu");
        public static readonly Key F1 = new Key("F1");
        public static readonly Key F2 = new Key("F2");
        public static readonly Key F3 = new Key("F3");
        public static readonly Key F4 = new Key("F4");
        public static readonly Key F5 = new Key("F5");
        public static readonly Key F6 = new Key("F6");
        public static readonly Key F7 = new Key("F7");
        public static readonly Key F8 = new Key("F8");
        public static readonly Key F9 = new Key("F9");
        public static readonly Key F10 = new Key("F10");
        public static readonly Key F11 = new Key("F11");
        public static readonly Key F12 = new Key("F12");
        public static readonly Key F13 = new Key("F13");
        public static readonly Key F14 = new Key("F14");
        public static readonly Key F15 = new Key("F15");
        public static readonly Key F16 = new Key("F16");
        public static readonly Key F17 = new Key("F17");
        public static readonly Key F18 = new Key("F18");
        public static readonly Key F19 = new Key("F19");
        public static readonly Key F20 = new Key("F20");
        public static readonly Key F21 = new Key("F21");
        public static readonly Key F22 = new Key("F22");
        public static readonly Key F23 = new Key("F23");
        public static readonly Key F24 = new Key("F24");
        public static readonly Key NumLock = new Key("NumLock");
        public static readonly Key ScrollLock = new Key("ScrollLock");
        public static readonly Key AudioVolumeMute = new Key("AudioVolumeMute");
        public static readonly Key AudioVolumeDown = new Key("AudioVolumeDown");
        public static readonly Key AudioVolumeUp = new Key("AudioVolumeUp");
        public static readonly Key MediaTrackNext = new Key("MediaTrackNext");
        public static readonly Key MediaTrackPrevious = new Key("MediaTrackPrevious");
        public static readonly Key MediaStop = new Key("MediaStop");
        public static readonly Key MediaPlayPause = new Key("MediaPlayPause");
        public static readonly Key Semicolon = new Key(";");
        public static readonly Key Comma = new Key(",");
        public static readonly Key EqualsSign = new Key("=");
        public static readonly Key PlusSign = new Key("+");
        public static readonly Key LesserThan = new Key("<");
        public static readonly Key MinusSign = new Key("-");
        public static readonly Key Underscore = new Key("_");
        public static readonly Key Period = new Key(".");
        public static readonly Key GreaterThan = new Key(">");
        public static readonly Key Slash = new Key("/");
        public static readonly Key QuestionMark = new Key("?");
        public static readonly Key Backquote = new Key("`");
        public static readonly Key Tilde = new Key("~");
        public static readonly Key OpenSquareBrackets = new Key("[");
        public static readonly Key OpenBrackets = new Key("{");
        public static readonly Key Pipe = new Key("|");
        public static readonly Key CloseSquareBrackets = new Key("]");
        public static readonly Key CloseBrackets = new Key("}");
        public static readonly Key Backslash = new Key("\\");
        public static readonly Key AltGraph = new Key("AltGraph");
        public static readonly Key Attn = new Key("Attn");
        public static readonly Key CrSel = new Key("CrSel");
        public static readonly Key ExSel = new Key("ExSel");
        public static readonly Key EraseEof = new Key("EraseEof");
        public static readonly Key Play = new Key("Play");
        public static readonly Key ZoomOut = new Key("ZoomOut");

        /// <inheritdoc />
        public override string ToString() => _value;

        /// <summary>
        /// Converts the <paramref name="key"/> to its underlining string value
        /// </summary>
        /// <param name="key">The key</param>
        public static implicit operator string(Key key) => key._value;
    }
}