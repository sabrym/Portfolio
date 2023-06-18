using System;
using System.ComponentModel;
using System.Data;
using ClosedXML.Excel;

namespace Portfolio.Services
{
	public static class ExcelReportWriter
	{
        public static Task WriteItemsToReport<T>(List<T> items, string reportName, string worksheetName)
        {
            using var workbook = new XLWorkbook();
            workbook.AddWorksheet(items.ToDataTable<T>(worksheetName)).ColumnsUsed().AdjustToContents();
            workbook.SaveAs($"{reportName}.xlsx");
            return Task.CompletedTask;
        }

        public static byte[] GenerateReport<T>(List<T> items, string reportName, string worksheetName)
        {
            using var workbook = new XLWorkbook();
            workbook.AddWorksheet(items.ToDataTable<T>(worksheetName)).ColumnsUsed().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, string name)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new(name);

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}

