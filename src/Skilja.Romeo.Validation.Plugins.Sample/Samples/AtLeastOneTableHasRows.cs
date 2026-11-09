using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Skilja.Romeo.Validation.Plugins.Sample.Samples
{
    /// <summary>
    /// Fails both assigned tables when both have zero rows; blank rows count as entries.
    /// </summary>
    /// <remarks>
    /// Upload the built sample DLL in Designer > Library > Plugins; install in other validation hosts as needed.
    /// Create one AtLeastOneTableHasRows multi-field rule assigned to exactly two tables (not rows/columns).
    /// Leave parameters, evaluation conditions and completions empty; disable ValidOnConfirm to enforce.
    /// Select both as Validates Fields. Error: "Enter a row in either table."
    /// </remarks>
    public class AtLeastOneTableHasRows : MultiFieldValidationBase
    {
        public override TargetFieldTypes TargetFieldTypesForConfig => TargetFieldTypes.Table;

        protected override Task<bool> InternalValidate(IValidatable validable, IValidationRuleDefinition definition)
        {
            var assigned = GetAssignedValidatables();
            if (assigned.Count != 2 || assigned.Values.Any(tables =>
                tables == null || tables.Count != 1 || tables[0] == null ||
                tables[0].ValidatableType != ValidatableType.Table))
            {
                throw new InvalidOperationException(
                    $"Rule {definition.Name} must be assigned to exactly two tables, with one instance of each.");
            }

            // Children are rows; the framework applies the result to both targets, respecting other rules.
            return Task.FromResult(assigned.Values.Any(tables => tables[0].GetChildren().Any()));
        }

        public override IList<string> GetDefaultParameters() => new List<string>();

        public override IList<string> GetDefaultCompletionConditions() => new List<string>();
    }

}
