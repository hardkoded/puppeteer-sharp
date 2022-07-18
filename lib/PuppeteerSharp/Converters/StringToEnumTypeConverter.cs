using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace CefSharp.DevTools.Dom.Converters
{
    internal class StringToEnumTypeConverter : EnumConverter
    {
        public StringToEnumTypeConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.IsEnum)
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.IsEnum)
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return string.Empty;
                }

                return StringToEnumInternal(destinationType, value.ToString());
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static object StringToEnumInternal(Type enumType, string input)
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == input)
                {
                    return Enum.Parse(enumType, name);
                }
            }

            return Enum.GetValues(enumType).GetValue(0);
        }
    }
}
