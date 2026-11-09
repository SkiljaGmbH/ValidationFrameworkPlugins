using Skilja.Romeo.Validation.Completion;
using Skilja.Romeo.Validation.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Skilja.Romeo.Validation.Plugins.Sample
{
    public class SampleCompletionMethod : CompletionBase
    {
        /// <summary>
        /// <paramref name="validatable"/> is the field (or table, row or cell) that is currently being validated.
        /// GetValidatableValue("fieldname") gets the text value of a given field
        /// SetValidatableValue("fieldname", "newTextValue") sets the text value of a given field
        /// SetValidatableValueWithSource("fieldname", "newTextValue", "source") sets the text value
        ///         of a given field and fills the CompletionState with the source information
        /// GetValidatableValues("cellName"), gets all cells of a given name, so you'd get all rows.
        /// </summary>
        /// <param name="validatable"></param>
        /// <returns></returns>
        protected override Task<bool> InternalComplete(IValidatable validatable)
        {
            var completionParameters = Definition.Parameters;
            // get the current text value of a field (in this sample, the first parameter specifies the first field to fetch)
            var fieldTextValue = base.GetValidatableValue(completionParameters[0]);
            var fieldTextValue2 = base.GetValidatableValue("myFieldName2");

            // this is the default patter for the CompletionState source name
            var sourceName = CompletionSourceNameGenerator.GetName(Definition.Name, nameof(SampleCompletionMethod));

            // set the text value of fieldToSet
            SetValidatableValueWithSource("fieldToSet", fieldTextValue + fieldTextValue2, sourceName);

            WorkingWithTables(validatable, false);

            var webserviceEndpoint = completionParameters[1]; // e.g. https://my.webservice.azure.com/api/parcels/
            int parcelId = int.TryParse(fieldTextValue, out var i) ? i : 5;

            // Do a web request. HttpClient would work too, but it requires to get everything back to synchronous contexts.
            // And we might be called from a WPF UI, so we'd have to take care of dead locks.
            WebRequest request = WebRequest.Create($"{webserviceEndpoint}{parcelId}");
            var response = request.GetResponse();

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                // Get the stream containing content returned by the server.  
                using (Stream dataStream = response.GetResponseStream())
                // Open the stream using a StreamReader for easy access.  
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    // Read the content.  
                    string responseFromServer = reader.ReadToEnd();
                    // set the content of another field to the web requests response.
                    SetValidatableValueWithSource(completionParameters[0], responseFromServer, sourceName);
                    return Task.FromResult(true);
                }
            }

            // return true if the completion changed something, false otherwise.
            return Task.FromResult(false);
        }

        private void WorkingWithTables(IValidatable validatable, bool run)
        {
            if (!run)
                return;
            // to get values that are in tables, GetValidatableValues can be used to get read access.
            // However, it is currently not possible to write to arbitrary rows in a table.
            string tableName = "myTable";
            string cellName = "myCell";
            List<IValidatableValue> allCells = GetValidatableValues($"{tableName}-TableRow|{cellName}").ToList();
            // allCells[0].Value - gets the text value
            // allCells[0].TypedValue - gets the typed value

            // in case the completion runs after a rule that is triggered by a row (e..g if the rule is triggered by myTable-TableRow), then the validatable is actually a row.
            // all child cells can be accessed in the following way:
            IList<IValidatable> cellsOfCurrentRow = validatable.GetChildren();

            // you could even go to the parent table like
            var parentTable = validatable.GetParentChain().First();
        }


    }
}