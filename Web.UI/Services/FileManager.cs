using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Web.UI.Services
{
    public static class FileManager
    {
        static string RepositoryPath = Path.Join(AppContext.BaseDirectory, "Documents");

        public static void CreateDocumentsFolder()
        {
            if (!Directory.Exists(RepositoryPath))
                Directory.CreateDirectory(RepositoryPath);
        }

        public static void SaveFile(IFormFile formFile, string tempFileName)
        {
            string saveFilePath = Path.Join(RepositoryPath, tempFileName);

            using (Stream fileContent = formFile.OpenReadStream())
            {
                using (FileStream fs = File.Create(saveFilePath))
                {
                    fileContent.Seek(0, SeekOrigin.Begin);
                    fileContent.CopyTo(fs);
                }
            }
        }

        public static void SaveReport(string reportData, string tempFileName)
        {
            string saveFileName = tempFileName.Substring(0, tempFileName.LastIndexOf(".")) + ".json";
            string saveFilePath = Path.Join(RepositoryPath, saveFileName);

            File.WriteAllText(saveFilePath, reportData);
        }

        public static string[] GetReportFiles()
        {
            string[] filePaths = Directory.GetFiles(RepositoryPath, "*.json");

            return filePaths;
        }

        public static string GetTempFileName(string NameWithExtension)
        {
            string nameWithoutExtension = NameWithExtension.Substring(0, NameWithExtension.LastIndexOf("."));
            string newNameWithoutExtension = string.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now);

            string newNameWithExtension = NameWithExtension.Replace(nameWithoutExtension, newNameWithoutExtension);

            return newNameWithExtension;
        }

        public static string ReadFile(IFormFile fl)
        {
            string fileContent = "";

            using (StreamReader reader = new StreamReader(fl.OpenReadStream()))
            {
                fileContent = reader.ReadToEnd();
            }

            return fileContent;
        }

        public static string ReadFile(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);

            return fileContent;
        }
    }
}
