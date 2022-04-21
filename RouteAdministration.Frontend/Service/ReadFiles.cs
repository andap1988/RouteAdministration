using Models;
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

        public static List<Person> ReadTXTCities(string pathWebRoot)
        {
            List<Person> cities = new();

            string folder = "\\File\\";
            string path = pathWebRoot + folder + "sp-city.txt";

            using (StreamReader reader = new(path))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    cities.Add(new Person { Name = line.Trim().Replace(",", "").Replace("\"", "") });
                    line = reader.ReadLine();
                }
            }

            return cities;
        }

        public static List<string> ReadHeaderExcelFile(string pathWebRoot)
        {
            List<string> headerExcel = new();
            string folder = "\\File\\";

            FileInfo excelFile = new FileInfo(pathWebRoot + folder + "Plan.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new(excelFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int cols = worksheet.Dimension.End.Column;

                for (int i = 1; i < cols; i++)
                {
                    headerExcel.Add(worksheet.Cells[1, i].Value.ToString());
                }
            }

            return headerExcel;
        }

        public static List<string> ReadServiceExcelFile(string pathWebRoot, string nameColumn)
        {
            List<string> services = new();

            string folder = "\\File\\"; ;

            FileInfo excelFile = new FileInfo(pathWebRoot + folder + "Plan.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new(excelFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int cols = worksheet.Dimension.End.Column;
                int rows = worksheet.Dimension.End.Row;

                for (int col = 1; col < cols; col++)
                {
                    if (worksheet.Cells[1, col].Value.ToString() == nameColumn)
                    {
                        for (int row = 2; row < rows; row++)
                        {
                            if (worksheet.Cells[row, col].Value != null)
                                services.Add(worksheet.Cells[row, col].Value.ToString());
                            else
                                break;
                        }
                        col = cols;
                    }
                }
            }

            return services;
        }

        public static List<IDictionary<string, string>> ReadExcelFile(List<string> columns, string pathWebRoot)
        {
            List<string> plan = new();
            string folder = "\\File\\";

            List<IDictionary<string, string>> listDictonary = new();

            FileInfo excelFile = new FileInfo(pathWebRoot + folder + "Plan.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new(excelFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int cols = worksheet.Dimension.End.Column;
                int rows = worksheet.Dimension.End.Row;

                IDictionary<string, string> data = new Dictionary<string, string>();

                for (int row = 2; row < rows; row++)
                {
                    data = new Dictionary<string, string>();

                    for (int col = 1; col < cols; col++)
                    {
                        columns.ForEach(column =>
                        {
                            if (worksheet.Cells[1, col].Value.ToString() == column)
                            {
                                if (worksheet.Cells[row, col].Value == null)
                                    data.Add(column, "");
                                else
                                    data.Add(column, worksheet.Cells[row, col].Value.ToString());
                            }
                        });

                    }
                    if (data.Count > 1) listDictonary.Add(data);
                }
            }

            return listDictonary;
        }

        public static List<string> ReadFileInFolder(string title, string pathWebRoot)
        {
            List<string> text = new();

            string fileName = title + ".txt";
            string folder = "\\File\\";
            string pathFinal = pathWebRoot + folder + fileName;

            using (StreamReader reader = new(pathFinal))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    text.Add(line);
                    line = reader.ReadLine();
                }
            }

            return text;
        }

        public static string ReturnForService(string title, string service, string pathWebRoot)
        {
            string results = "";

            string fileName = title + ".txt";
            string folder = "\\File\\";
            string pathFinal = pathWebRoot + folder + fileName;

            using (StreamReader reader = new(pathFinal))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var a = line.Split(';');
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == service)
                        {
                            results = results + line + "\n";
                        }
                    }
                    line = reader.ReadLine();
                }
            }

            return results;
        }
    }
}
