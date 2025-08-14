using Skilja.Romeo.Validation.Interfaces;
using Skilja.Romeo.Validation.Validations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skilja.Romeo.Validation.Plugins.Sample
{
    public class SampleMultiFieldValidationRule : MultiFieldValidationBase
    {
        private IValidationRuleDefinition _definition;

        protected override Task<bool> InternalValidate(IValidatable validatable, IValidationRuleDefinition definition)
        {
            // A 'validatable' is a field that initially triggered a rule.
            // The GetValidatables(fieldName) method retrieves all validatables with the specified name.
            // In the case of tables and rows, there can be more than one. The assigned fields configured
            // for this rule determine which validatables are available to the GetValidatables method.
            // If you want to work on a specific row of a table, assign the row, not the cells, to the validation rule.
            // This ensures that you only get validatables for that row.

            // In this example, we compare the sum of certain fields to the expected sum configured in the parameters.
            _definition = definition;

            // 1. Parse and validate parameters
            var parameters = ParseParameters(_definition);

            // 2. Get Field Values: Please note that we use the converted value here, which is set by the formatter.
            var fieldValues = GetFieldValues(parameters);

            // 3. Calculate the sum
            var sum = fieldValues.Sum();

            // 4. Finally, we check if the sum is the value configured in the parameters.
            var expectedSum = parameters.ExpectedSum;

            return Task.FromResult(expectedSum == sum);
        }

        // It's always a good idea to check the parameters before executing rule logic.
        private SampleMultiFieldValidationRuleParameters ParseParameters(IValidationRuleDefinition definition)
        {
            // Parameters are a list of strings, and two entries are required for this rule to be configured correctly. For example:
            //      "FieldA|FieldB|FieldC"
            //      "15"
            // That means FieldA + FieldB + FieldC = 15 must be true for this rule to be valid.
            if (definition.Parameters.Count() != 2)
                throw new InvalidOperationException($"The rule {definition.Name} has no correct parameters defined.");

            var fields = definition.Parameters.First().Split('|').ToList();
            var expectedValueString = definition.Parameters.Skip(1).First();

            if (!decimal.TryParse(expectedValueString, out var expectedValue))
                throw new InvalidOperationException($"The expected sum configured in the parameters of rul {definition.Name} cannot be parsed: {expectedValueString}.");

            var parameters = new SampleMultiFieldValidationRuleParameters()
            {
                FieldNames = fields,
                ExpectedSum = expectedValue
            };

            return parameters;
        }

        // This can obviously be done more compactly, but it is important to be very careful when parsing or using converted values.
        // Otherwise, an unexpected field type could cause the rule to break.
        private List<decimal> GetFieldValues(SampleMultiFieldValidationRuleParameters parameters)
        {
            var fieldValues = new List<decimal>();
            foreach (var field in parameters.FieldNames)
            {
                var validatable = GetValidatables(field).FirstOrDefault();
                if (validatable != null)
                    fieldValues.Add(GetDecimalValue(validatable));
            }

            return fieldValues;
        }

        // This converts a typed field value to a decimal value. We expect strings or types that can be converted directly.
        // This is usually sufficient, but if you have any doubts, add more robust checks to your rule when working with typed values.
        public static decimal GetDecimalValue(IValidatable validatable)
        {
            var convertedValue = validatable.GetConvertedValue();
            if (convertedValue == null || (convertedValue is string convertedString && string.IsNullOrWhiteSpace(convertedString)))
                return 0;

            return Convert.ToDecimal(convertedValue);
        }

        public override IList<string> GetDefaultCompletionConditions()
        {
            // also empty, will be deprecated
            return new List<string>();
        }

        public override IList<string> GetDefaultParameters()
        {
            // should not do anything.
            return new List<string>();
        }

        private class SampleMultiFieldValidationRuleParameters
        {
            public List<string> FieldNames { get; set; } = new List<string>();
            public decimal ExpectedSum { get; set; }
        }
    }
}
