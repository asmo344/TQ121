using OfficeOpenXml;

namespace System
{
    public static class MyExcel
    {
        public static void WriteToColumn(this ExcelWorksheet sheet, int column, params object[] value)
        {
            WriteToColumn(sheet, column, 0, false, value);
        }

        public static void WriteToColumn(this ExcelWorksheet sheet, int column, bool bold, params object[] value)
        {
            WriteToColumn(sheet, column, 0, bold, value);
        }

        public static void WriteToColumn(this ExcelWorksheet sheet, int column, int rowOffset, params object[] value)
        {
            WriteToColumn(sheet, column, rowOffset, false, value);
        }

        public static void WriteToColumn(this ExcelWorksheet sheet, int column, int rowOffset, bool bold, params object[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                sheet.Cells[i + 1 + rowOffset, column].Value = value[i];
                sheet.Cells[i + 1 + rowOffset, column].Style.Font.Bold = bold;
            }
        }

        public static void WriteToRow(this ExcelWorksheet sheet, int row, params object[] value)
        {
            WriteToRow(sheet, row, 0, false, value);
        }

        public static void WriteToRow(this ExcelWorksheet sheet, int row, bool bold, params object[] value)
        {
            WriteToRow(sheet, row, 0, bold, value);
        }

        public static void WriteToRow(this ExcelWorksheet sheet, int row, int columnOffset, params object[] value)
        {
            WriteToRow(sheet, row, columnOffset, false, value);
        }

        public static void WriteToRow(this ExcelWorksheet sheet, int row, int columnOffset, bool bold, params object[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                sheet.Cells[row, i + 1 + columnOffset].Value = value[i];
                sheet.Cells[row, i + 1 + columnOffset].Style.Font.Bold = bold;
            }
        }
    }
}