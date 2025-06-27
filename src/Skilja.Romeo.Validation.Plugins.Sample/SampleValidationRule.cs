using Skilja.Romeo.Validation.Interfaces;
using Skilja.Romeo.Validation.Validations.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skilja.Romeo.Validation.Plugins.Sample
{
    /// <summary>
    /// Sample single field validation rule
    /// </summary>
    public class SampleValidationRule : ValidationBase
    {
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

        protected override Task<bool> InternalValidate(IValidatable field, IValidationRuleDefinition definition)
        {
            // validation logic
            var textValue = field.GetValue();
            var convertedValue = field.GetConvertedValue();
            if (textValue == "i am correct" || convertedValue.GetType() == typeof(decimal) && (decimal)convertedValue == 33)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
