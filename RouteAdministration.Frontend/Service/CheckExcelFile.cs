using Microsoft.AspNetCore.Http;

namespace RouteAdministration.Frontend.Service
{
    public class CheckExcelFile
    {
        public static bool IsExcel(IFormFile file)
        {
            var extension = "." + file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
            return (extension == ".xlsx" || extension == ".xls");
        }
    }
}
