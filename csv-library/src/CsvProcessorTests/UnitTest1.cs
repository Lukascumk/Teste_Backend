using System;
using Xunit;
using CsvProcessorCOMLibrary;

namespace CsvProcessorTests
{
    public class CsvProcessorTests
    {
        [Fact]
        public void ProcessCsv_ShouldReturnFilteredAndSelectedColumns()
        {
            
            var csvProcessor = new CsvProcessor();
            string csv = "header1,header2,header3\n1,2,3\n4,5,6\n7,8,9";
            string selectedColumns = "header1,header3";
            string filters = "header1>1\nheader3<9";

            
            string result = csvProcessor.ProcessCsv(csv, selectedColumns, filters);

            
            string expected = "header1,header3\n4,6\n";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ProcessCsvFile_ShouldReturnFilteredAndSelectedColumns()
        {
            
            var csvProcessor = new CsvProcessor();
            string csvFilePath = "test.csv";
            string selectedColumns = "header1,header3";
            string filters = "header1>1\nheader3<9";

            
            System.IO.File.WriteAllText(csvFilePath, "header1,header2,header3\n1,2,3\n4,5,6\n7,8,9");

            
            string result = csvProcessor.ProcessCsvFile(csvFilePath, selectedColumns, filters);

            
            string expected = "header1,header3\n4,6\n";
            Assert.Equal(expected, result);

            
            System.IO.File.Delete(csvFilePath);
        }
    }
}
