using Skilja.Romeo.Validation.Interfaces;
using System;
using System.Collections.Generic;

namespace Skilja.Romeo.Validation.Plugins.Sample
{
    public class AccessingLookups
    {
        /// <summary>
        /// This is the same property that validation rules have. Validation rules and completion methods can use that property to get a lookup.
        /// If the lookup doesn't exist (not configured via a lookups configuration), the null will be returned.
        /// </summary>
        public Func<string, ILookup> GetLookup { get; set; }

        /// <summary>
        /// Searches the lookup called <paramref name="lookupName"/> for <paramref name="searchText"/>.
        /// If any resulting row contains the <paramref name="searchText"/>, true is returned, false otherwise.
        /// </summary>
        public bool LookupHasValue(string lookupName, string searchText)
        {
            // get the lookup, we assume that it exists.
            var lookup = GetLookup(lookupName);

            // the column that you want to compare is usually configured via a parameter of the rule/method. You can of course use multiple columns.
            var myLookupColumn = "Street";

            var lookupQuery = new LookupQuery { SearchText = searchText };

            int selectedColumn = GetLookupColumnIndex(lookup, myLookupColumn);
            if (lookup.ColumnNames.Length > selectedColumn)
            {
                string columnName = lookup.ColumnNames[selectedColumn];
                // Only affects static lookups so far (not e.g. restful lookups)
                // If the filter column is set, that column must contain the Filter text. Otherwise, all columns will be compared.
                lookupQuery.FilterColumns = new List<string> { columnName };
            }

            // search the lookup
            var results = lookup.Search(lookupQuery).GetAwaiter().GetResult();

            if (results == null || string.IsNullOrEmpty(lookup.LookupError) == false)
            {
                // this can happen if the lookup can't access the lookup service, or encounters some other error.
                // you can see the error in LookupError
                return false;
            }

            // if nothing was found, there's nothing to do
            if (results.Rows.Count == 0)
                return false;

            for (int rowIndex = 0; rowIndex < results.Rows.Count; rowIndex++)
            {
                // you can now get the content of each column. Since the result is of type object, you'll want to convert that to a string
                var columnResult = results.Rows[rowIndex].Entries[selectedColumn];
                if (columnResult != null && columnResult.Contains(searchText))
                {
                    // lets return true if the text you're searching is contained in the lookup
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the index of the column by the <paramref name="columnName"/>. If the name cannot be found and <paramref name="columnName"/> is a number,
        /// it will return the integer value of that number.
        /// </summary>
        internal static int GetLookupColumnIndex(ILookup lookup, string columnName)
        {
            var index = Array.IndexOf(lookup.ColumnNames, columnName);
            // if index cannot be found, we might be dealing with old-style static lookups. Then the column name might be an index.
            if (index == -1 && int.TryParse(columnName, out var num))
            {
                index = num;
            }

            return index;
        }
    }
}