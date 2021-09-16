using System;
using System.IO;
using System.Security.Cryptography;
using Xamarin.Forms;

namespace Autorentool_RMT.Services
{
    public class FileHandler
    {
        #region CreateDirectory
        /// <summary>
        /// Creates directory by given directoryName and returns the full path.
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public static string CreateDirectory(string directoryName)
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    directoryName
                    )
                );

            return directoryInfo.FullName;
        }
        #endregion

        #region GetUniqueFilename
        /// <summary>
        /// Returns unqiue filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetUniqueFilename(string filename, string directoryPath)
        {
            int filenameIncrement = 1;

            while (File.Exists(Path.Combine(directoryPath, filename)))
            {
                int pointIndex = filename.LastIndexOf(".");

                string filenameWithoutFiletype = filename.Substring(0, pointIndex) + filenameIncrement;
                string filetype = ExtractFiletypeFromPath(filename);

                filename = filenameWithoutFiletype + "." + filetype;
                filenameIncrement++;
            }

            return filename;
        }
        #endregion

        #region SaveFile
        /// <summary>
        /// Saves given stream as file under given filePath.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        public static void SaveFile(Stream stream, string filePath)
        {
            try
            {

                byte[] bArray = new byte[stream.Length];

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    stream.Read(bArray, 0, (int)stream.Length);
                    int length = bArray.Length;
                    fs.Write(bArray, 0, length);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetFileHashAsString
        /// <summary>
        /// Computes a sha512-hash from stream and returns it as string.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetFileHashAsString(Stream stream)
        {
            string hashAsString = "";
            byte[] bytes = new byte[stream.Length];

            using (SHA512 sha512 = new SHA512Managed())
            {
                stream.Read(bytes, 0, (int)stream.Length);
                byte[] hashCode = sha512.ComputeHash(bytes);
                hashAsString = BitConverter.ToString(hashCode).Replace("-", "");
            }

            return hashAsString;
        }
        #endregion

        #region ExtractFiletypeFromPath
        /// <summary>
        /// Extracts the filetype from path starting from the last slash.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ExtractFiletypeFromPath(string path)
        {
            int filetypeIndex = path.LastIndexOf(".");
            int length = path.Length - filetypeIndex;
            string filetype = path.Substring(filetypeIndex + 1, length - 1);
            return filetype;
        }
        #endregion

        #region GetImageSource
        public static ImageSource GetImageSource(string path)
        {
           return ImageSource.FromStream(() => 
           {
               Stream stream = File.OpenRead(path);
               return stream;
           });
        }
        #endregion

    }

}
