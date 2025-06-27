using System;
using Skilja.Romeo.Validation.Formatting;

namespace Skilja.Romeo.Validation.Plugins.Sample
{
    public class SampleFormatter : BaseFormatter
    {
        public SampleFormatter()
        {
            TargetType = typeof(Decimal);
        }

        public override bool ConvertToType(string stringValue, out object convertedValue)
        {
            // this will get the text as input and convert it to the type of this formatter.
            // e.g., a decimal formatter needs to parse the text and produce a proper decimal value
            // if the text cannot be parsed (e.g. 'not a decimal') then this method should return false.
            decimal properDecimalValue = 33;
            convertedValue = properDecimalValue;
            return true;
        }
        public override bool FormatStringValue(object convertedValue, out string stringValue)
        {
            // Null values mean that the typed value was not set to a specific value
            // which is usually the case when the ConvertToType could not parse the given string value.
            // In this case return false, so that the text value will not be overwritten by the stringValue output.
            if (convertedValue == null)
            {
                stringValue = null;
                return false;
            }

            // this will take the parsed value and format it according to what the text should look like
            stringValue = convertedValue.ToString();
            return true;
        }

        public override Type GetFormatterType()
        {
            // this is a formatter for decimal values
            return typeof(decimal);
        }
    }
}