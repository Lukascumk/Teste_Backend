using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CsvProcessorCOMLibrary
{
    [ComVisible(true)]
    [Guid("E7B20E7E-4D71-4473-83FA-8B724276D5E0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICsvProcessor
    {
        string ProcessCsv(string csv, string selectedColumns, string rowFilterDefinitions);
        string ProcessCsvFile(string csvFilePath, string selectedColumns, string rowFilterDefinitions);
    }

    [ComVisible(true)]
    [Guid("6D1B17F3-C5FF-4F97-90B6-29F4F68B1EA2")]
    [ClassInterface(ClassInterfaceType.None)]
    public class CsvProcessor : ICsvProcessor
    {
        public string ProcessCsv(string csv, string selectedColumns, string rowFilterDefinitions)
        {
            var lines = csv.Split('\n');
            var headers = lines[0].Split(',');

            var selectedIndices = GetSelectedIndices(headers, selectedColumns);
            var filters = ParseFilters(rowFilterDefinitions, headers);

            var output = new StringBuilder();
            output.AppendLine(string.Join(",", selectedIndices.Select(i => headers[i])));

            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                if (ApplyFilters(values, filters))
                {
                    output.AppendLine(string.Join(",", selectedIndices.Select(i => values[i])));
                }
            }

            return output.ToString();
        }

        public string ProcessCsvFile(string csvFilePath, string selectedColumns, string rowFilterDefinitions)
        {
            var csv = File.ReadAllText(csvFilePath);
            return ProcessCsv(csv, selectedColumns, rowFilterDefinitions);
        }

        private static List<int> GetSelectedIndices(string[] headers, string selectedColumns)
        {
            if (string.IsNullOrEmpty(selectedColumns))
                return Enumerable.Range(0, headers.Length).ToList();

            var selected = selectedColumns.Split(',');
            return headers
                .Select((header, index) => new { header, index })
                .Where(x => selected.Contains(x.header))
                .Select(x => x.index)
                .ToList();
        }

        private static List<Func<string[], bool>> ParseFilters(string rowFilterDefinitions, string[] headers)
        {
            if (string.IsNullOrEmpty(rowFilterDefinitions))
                return new List<Func<string[], bool>>();

            var filters = new List<Func<string[], bool>>();
            var filterDefinitions = rowFilterDefinitions.Split('\n');

            foreach (var filterDefinition in filterDefinitions)
            {
                var parts = filterDefinition.Split(new char[] { '=', '<', '>' }, 2);
                var header = parts[0];
                var value = parts[1];
                var index = Array.IndexOf(headers, header);

                if (filterDefinition.Contains('='))
                    filters.Add(values => string.Compare(values[index], value) == 0);
                else if (filterDefinition.Contains('>'))
                    filters.Add(values => string.Compare(values[index], value) > 0);
                else if (filterDefinition.Contains('<'))
                    filters.Add(values => string.Compare(values[index], value) < 0);
            }

            return filters;
        }

        private static bool ApplyFilters(string[] values, List<Func<string[], bool>> filters)
        {
            return filters.All(filter => filter(values));
        }
    }
}
