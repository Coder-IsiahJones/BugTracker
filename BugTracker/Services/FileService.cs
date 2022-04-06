using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class FileService : IFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        #region ConvertByteArrayToFile

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);

                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion ConvertByteArrayToFile

        #region ConvertFileToByteArrayAsync

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                memoryStream.Close();
                memoryStream.Dispose();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion ConvertFileToByteArrayAsync

        #region FormatFileSize

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }

        #endregion FormatFileSize

        #region GetFileIcon

        //public string GetFileIcon(string file)
        //{
        //    string fileImage = "default";

        //    if (!string.IsNullOrWhiteSpace(file))
        //    {
        //        fileImage = Path.GetExtension(file).Replace(".", "");

        //        return $"/img/png/{fileImage}.png";
        //    }

        //    return fileImage;
        //}

        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/contentType/{ext}.png";
        }

        #endregion GetFileIcon
    }
}