using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class WriteFiles
    {
        public static async Task<bool> WriteFileInFolder(IFormFile file, string pathWebRoot)
        {
            bool isSaveSuccess = false;
            string fileName;

            try
            {
                //var extension = "." + file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
                fileName = "Plan.xlsx";

                string folder = "\\File\\";
                string pathFinal = pathWebRoot + folder + fileName;

                using (var stream = new FileStream(pathFinal, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                isSaveSuccess = true;

                return isSaveSuccess;
            }
            catch (Exception exception)
            {
                return isSaveSuccess;
            }
        }

        public static void WriteStringInFolder(List<string> text = null, string singleString = null, string title = "a", string pathWebRoot = "")
        {
            string fileName = title + ".txt";
            string folder = "\\File\\";
            string pathFinal = pathWebRoot + folder + fileName;

            if (text != null)
            {
                using (StreamWriter writer = new(pathFinal))
                {
                    text.ForEach(textUnique =>
                    {
                        writer.WriteLine(textUnique);

                    });
                }
            }
            else if (singleString != null)
            {
                using (StreamWriter writer = new(pathFinal))
                {
                    writer.WriteLine(singleString);
                }
            }
        }
    }
}
