using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace RouteAdministration.Frontend.Service
{
    public class ReadFiles
    {
        public static bool IsValid(string title, string extension, string pathWebRoot)
        {
            string fileName = title + extension;
            string folder = "\\File\\";
            string pathFinal = pathWebRoot + folder + fileName;
            bool exists = false;

            if (File.Exists(pathFinal))
                exists = true;

            return exists;
        }

        public static void ReOrderExcel(string title, string extension, string pathWebRoot)
        {
            string fileName = title + extension;
            string folder = "\\File\\";
            string pathFinal = pathWebRoot + folder + fileName;

            List<string> headerExcel = new();

            FileInfo excelFile = new FileInfo(pathWebRoot + folder + "Plan.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new(excelFile))
            {
                ExcelWorkbook wb = package.Workbook;

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int cols = worksheet.Dimension.End.Column;
                int rows = worksheet.Dimension.End.Row;
                int colCep = 0;

                for (int col = 1; col <= cols; col++)
                {
                    if (worksheet.Cells[1, col].Value.ToString().ToUpper() == "CEP")
                    {
                        colCep = col - 1;
                        break;
                    }
                }

                worksheet.Cells[2, 1, rows, cols].Sort(colCep, false);              

                package.Save();
            }
        }
    }
}
