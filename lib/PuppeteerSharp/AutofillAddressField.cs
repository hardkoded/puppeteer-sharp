namespace PuppeteerSharp
{
    /// <summary>
    /// Supported autofill address field names.
    /// See https://source.chromium.org/chromium/chromium/src/+/main:components/autofill/core/browser/field_types.cc
    /// for the full list of supported fields.
    /// </summary>
    public static class AutofillAddressField
    {
        /// <summary>First name.</summary>
        public const string NameFirst = "NAME_FIRST";

        /// <summary>Middle name.</summary>
        public const string NameMiddle = "NAME_MIDDLE";

        /// <summary>Last name.</summary>
        public const string NameLast = "NAME_LAST";

        /// <summary>Full name.</summary>
        public const string NameFull = "NAME_FULL";

        /// <summary>Email address.</summary>
        public const string EmailAddress = "EMAIL_ADDRESS";

        /// <summary>Phone home number.</summary>
        public const string PhoneHomeNumber = "PHONE_HOME_NUMBER";

        /// <summary>Phone home city and number.</summary>
        public const string PhoneHomeCityAndNumber = "PHONE_HOME_CITY_AND_NUMBER";

        /// <summary>Phone home whole number.</summary>
        public const string PhoneHomeWholeNumber = "PHONE_HOME_WHOLE_NUMBER";

        /// <summary>Address home line 1.</summary>
        public const string AddressHomeLine1 = "ADDRESS_HOME_LINE1";

        /// <summary>Address home line 2.</summary>
        public const string AddressHomeLine2 = "ADDRESS_HOME_LINE2";

        /// <summary>Address home street address.</summary>
        public const string AddressHomeStreetAddress = "ADDRESS_HOME_STREET_ADDRESS";

        /// <summary>Address home city.</summary>
        public const string AddressHomeCity = "ADDRESS_HOME_CITY";

        /// <summary>Address home state.</summary>
        public const string AddressHomeState = "ADDRESS_HOME_STATE";

        /// <summary>Address home zip code.</summary>
        public const string AddressHomeZip = "ADDRESS_HOME_ZIP";

        /// <summary>Address home country.</summary>
        public const string AddressHomeCountry = "ADDRESS_HOME_COUNTRY";
    }
}
