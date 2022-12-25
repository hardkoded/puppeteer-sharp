namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Utility class to be used with <see cref="Keyboard"/> operations.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Cancel key.
        /// </summary>
        public static readonly Key Cancel = new Key("Cancel");

        /// <summary>
        /// Help key.
        /// </summary>
        public static readonly Key Help = new Key("Help");

        /// <summary>
        /// Backspace key.
        /// </summary>
        public static readonly Key Backspace = new Key("Backspace");

        /// <summary>
        /// Tab key.
        /// </summary>
        public static readonly Key Tab = new Key("Tab");

        /// <summary>
        /// Clear key.
        /// </summary>
        public static readonly Key Clear = new Key("Clear");

        /// <summary>
        /// Enter key.
        /// </summary>
        public static readonly Key Enter = new Key("Enter");

        /// <summary>
        /// Shift key.
        /// </summary>
        public static readonly Key Shift = new Key("Shift");

        /// <summary>
        /// Control key.
        /// </summary>
        public static readonly Key Control = new Key("Control");

        /// <summary>
        /// Alt key.
        /// </summary>
        public static readonly Key Alt = new Key("Alt");

        /// <summary>
        /// Pause key.
        /// </summary>
        public static readonly Key Pause = new Key("Pause");

        /// <summary>
        /// CapsLock key.
        /// </summary>
        public static readonly Key CapsLock = new Key("CapsLock");

        /// <summary>
        /// Escape key.
        /// </summary>
        public static readonly Key Escape = new Key("Escape");

        /// <summary>
        /// Convert key.
        /// </summary>
        public static readonly Key Convert = new Key("Convert");

        /// <summary>
        /// NonConvert key.
        /// </summary>
        public static readonly Key NonConvert = new Key("NonConvert");

        /// <summary>
        /// Accept key.
        /// </summary>
        public static readonly Key Accept = new Key("Accept");

        /// <summary>
        /// ModeChange key.
        /// </summary>
        public static readonly Key ModeChange = new Key("ModeChange");

        /// <summary>
        /// PageUp key.
        /// </summary>
        public static readonly Key PageUp = new Key("PageUp");

        /// <summary>
        /// PageDown key.
        /// </summary>
        public static readonly Key PageDown = new Key("PageDown");

        /// <summary>
        /// End key.
        /// </summary>
        public static readonly Key End = new Key("End");

        /// <summary>
        /// Home key.
        /// </summary>
        public static readonly Key Home = new Key("Home");

        /// <summary>
        /// ArrowLeft key.
        /// </summary>
        public static readonly Key ArrowLeft = new Key("ArrowLeft");

        /// <summary>
        /// ArrowUp key.
        /// </summary>
        public static readonly Key ArrowUp = new Key("ArrowUp");

        /// <summary>
        /// ArrowRight key.
        /// </summary>
        public static readonly Key ArrowRight = new Key("ArrowRight");

        /// <summary>
        /// ArrowDown key.
        /// </summary>
        public static readonly Key ArrowDown = new Key("ArrowDown");

        /// <summary>
        /// Select key.
        /// </summary>
        public static readonly Key Select = new Key("Select");

        /// <summary>
        /// Print key.
        /// </summary>
        public static readonly Key Print = new Key("Print");

        /// <summary>
        /// Execute key.
        /// </summary>
        public static readonly Key Execute = new Key("Execute");

        /// <summary>
        /// PrintScreen key.
        /// </summary>
        public static readonly Key PrintScreen = new Key("PrintScreen");

        /// <summary>
        /// Insert key.
        /// </summary>
        public static readonly Key Insert = new Key("Insert");

        /// <summary>
        /// Delete key.
        /// </summary>
        public static readonly Key Delete = new Key("Delete");

        /// <summary>
        /// ')' key.
        /// </summary>
        public static readonly Key CloseParentheses = new Key(")");

        /// <summary>
        /// '!' key.
        /// </summary>
        public static readonly Key ExclamationMark = new Key("!");

        /// <summary>
        /// '@' key.
        /// </summary>
        public static readonly Key AtSign = new Key("@");

        /// <summary>
        /// '#' key.
        /// </summary>
        public static readonly Key NumberSign = new Key("#");

        /// <summary>
        /// '$' key.
        /// </summary>
        public static readonly Key DollarSign = new Key("$");

        /// <summary>
        /// '%' key.
        /// </summary>
        public static readonly Key Percent = new Key("%");

        /// <summary>
        /// '^' key.
        /// </summary>
        public static readonly Key Caret = new Key("^");

        /// <summary>
        /// <![CDATA['&']]> key.
        /// </summary>
        public static readonly Key Ampersand = new Key("&");

        /// <summary>
        /// Asterisk key.
        /// </summary>
        public static readonly Key Asterisk = new Key("*");

        /// <summary>
        /// '(' key.
        /// </summary>
        public static readonly Key OpenParentheses = new Key("(");

        /// <summary>
        /// Meta key.
        /// </summary>
        public static readonly Key Meta = new Key("Meta");

        /// <summary>
        /// ContextMenu key.
        /// </summary>
        public static readonly Key ContextMenu = new Key("ContextMenu");

        /// <summary>
        /// F1 key.
        /// </summary>
        public static readonly Key F1 = new Key("F1");

        /// <summary>
        /// F2 key.
        /// </summary>
        public static readonly Key F2 = new Key("F2");

        /// <summary>
        /// F3 key.
        /// </summary>
        public static readonly Key F3 = new Key("F3");

        /// <summary>
        /// F4 key.
        /// </summary>
        public static readonly Key F4 = new Key("F4");

        /// <summary>
        /// F5 key.
        /// </summary>
        public static readonly Key F5 = new Key("F5");

        /// <summary>
        /// F6 key.
        /// </summary>
        public static readonly Key F6 = new Key("F6");

        /// <summary>
        /// F7 key.
        /// </summary>
        public static readonly Key F7 = new Key("F7");

        /// <summary>
        /// F8 key.
        /// </summary>
        public static readonly Key F8 = new Key("F8");

        /// <summary>
        /// F9 key.
        /// </summary>
        public static readonly Key F9 = new Key("F9");

        /// <summary>
        /// F10 key.
        /// </summary>
        public static readonly Key F10 = new Key("F10");

        /// <summary>
        /// F11 key.
        /// </summary>
        public static readonly Key F12 = new Key("F12");

        /// <summary>
        /// F12 key.
        /// </summary>
        public static readonly Key F11 = new Key("F11");

        /// <summary>
        /// F13 key.
        /// </summary>
        public static readonly Key F13 = new Key("F13");

        /// <summary>
        /// F14 key.
        /// </summary>
        public static readonly Key F14 = new Key("F14");

        /// <summary>
        /// F15 key.
        /// </summary>
        public static readonly Key F15 = new Key("F15");

        /// <summary>
        /// F16 key.
        /// </summary>
        public static readonly Key F16 = new Key("F16");

        /// <summary>
        /// F17 key.
        /// </summary>
        public static readonly Key F17 = new Key("F17");

        /// <summary>
        /// F18 key.
        /// </summary>
        public static readonly Key F18 = new Key("F18");

        /// <summary>
        /// F19 key.
        /// </summary>
        public static readonly Key F19 = new Key("F19");

        /// <summary>
        /// F20 key.
        /// </summary>
        public static readonly Key F20 = new Key("F20");

        /// <summary>
        /// F21 key.
        /// </summary>
        public static readonly Key F21 = new Key("F21");

        /// <summary>
        /// F22 key.
        /// </summary>
        public static readonly Key F22 = new Key("F22");

        /// <summary>
        /// F23 key.
        /// </summary>
        public static readonly Key F23 = new Key("F23");

        /// <summary>
        /// F24 key.
        /// </summary>
        public static readonly Key F24 = new Key("F24");

        /// <summary>
        /// NumLock key.
        /// </summary>
        public static readonly Key NumLock = new Key("NumLock");

        /// <summary>
        /// ScrollLock key.
        /// </summary>
        public static readonly Key ScrollLock = new Key("ScrollLock");

        /// <summary>
        /// AudioVolumeMute key.
        /// </summary>
        public static readonly Key AudioVolumeMute = new Key("AudioVolumeMute");

        /// <summary>
        /// AudioVolumeDown key.
        /// </summary>
        public static readonly Key AudioVolumeDown = new Key("AudioVolumeDown");

        /// <summary>
        /// AudioVolumeUp key.
        /// </summary>
        public static readonly Key AudioVolumeUp = new Key("AudioVolumeUp");

        /// <summary>
        /// MediaTrackNext key.
        /// </summary>
        public static readonly Key MediaTrackNext = new Key("MediaTrackNext");

        /// <summary>
        /// MediaTrackPrevious key.
        /// </summary>
        public static readonly Key MediaTrackPrevious = new Key("MediaTrackPrevious");

        /// <summary>
        /// MediaStop key.
        /// </summary>
        public static readonly Key MediaStop = new Key("MediaStop");

        /// <summary>
        /// MediaPlayPause key.
        /// </summary>
        public static readonly Key MediaPlayPause = new Key("MediaPlayPause");

        /// <summary>
        /// ';' key.
        /// </summary>
        public static readonly Key Semicolon = new Key(";");

        /// <summary>
        /// ',' key.
        /// </summary>
        public static readonly Key Comma = new Key(",");

        /// <summary>
        /// '=' key.
        /// </summary>
        public static readonly Key EqualsSign = new Key("=");

        /// <summary>
        /// '+' key.
        /// </summary>
        public static readonly Key PlusSign = new Key("+");

        /// <summary>
        /// <![CDATA['<']]> key.
        /// </summary>
        public static readonly Key LesserThan = new Key("<");

        /// <summary>
        /// '-' key.
        /// </summary>
        public static readonly Key MinusSign = new Key("-");

        /// <summary>
        /// '_' key.
        /// </summary>
        public static readonly Key Underscore = new Key("_");

        /// <summary>
        /// '.' key.
        /// </summary>
        public static readonly Key Period = new Key(".");

        /// <summary>
        /// <![CDATA['>']]> key.
        /// </summary>
        public static readonly Key GreaterThan = new Key(">");

        /// <summary>
        /// <![CDATA['/']]>key.
        /// </summary>
        public static readonly Key Slash = new Key("/");

        /// <summary>
        /// '?' key.
        /// </summary>
        public static readonly Key QuestionMark = new Key("?");

        /// <summary>
        /// '`' key.
        /// </summary>
        public static readonly Key Backquote = new Key("`");

        /// <summary>
        /// '~' key.
        /// </summary>
        public static readonly Key Tilde = new Key("~");

        /// <summary>
        /// <![CDATA['[']]> key.
        /// </summary>
        public static readonly Key OpenSquareBrackets = new Key("[");

        /// <summary>
        /// '{' key.
        /// </summary>
        public static readonly Key OpenBrackets = new Key("{");

        /// <summary>
        /// <![CDATA[']']]> key.
        /// </summary>
        public static readonly Key CloseSquareBrackets = new Key("]");

        /// <summary>
        /// '|' key.
        /// </summary>
        public static readonly Key Pipe = new Key("|");

        /// <summary>
        /// '}' key.
        /// </summary>
        public static readonly Key CloseBrackets = new Key("}");

        /// <summary>
        /// '\' key.
        /// </summary>
        public static readonly Key Backslash = new Key("\\");

        /// <summary>
        /// AltGraph key.
        /// </summary>
        public static readonly Key AltGraph = new Key("AltGraph");

        /// <summary>
        /// Attn key.
        /// </summary>
        public static readonly Key Attn = new Key("Attn");

        /// <summary>
        /// CrSel key.
        /// </summary>
        public static readonly Key CrSel = new Key("CrSel");

        /// <summary>
        /// ExSel key.
        /// </summary>
        public static readonly Key ExSel = new Key("ExSel");

        /// <summary>
        /// EraseEof key.
        /// </summary>
        public static readonly Key EraseEof = new Key("EraseEof");

        /// <summary>
        /// Play key.
        /// </summary>
        public static readonly Key Play = new Key("Play");

        /// <summary>
        /// ZoomOut key.
        /// </summary>
        public static readonly Key ZoomOut = new Key("ZoomOut");

        private readonly string _value;

        private Key(string value) => _value = value;

        /// <summary>
        /// Converts the <paramref name="key"/> to its underlining string value.
        /// </summary>
        /// <param name="key">The key.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Exceptions should not be raised in this type of method.")]
        public static implicit operator string(Key key)
        {
            return key._value;
        }

        /// <inheritdoc />
        public override string ToString() => _value;
    }
}
