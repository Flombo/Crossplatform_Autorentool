using SkiaSharp;
using System;
using System.Collections.Generic;
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
            string directoryPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                    ), 
                directoryName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
        #endregion

        #region GetUniqueFilename
        /// <summary>
        /// Returns unique filename.
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
                byte[] thumbnailImageBytes = new byte[stream.Length];

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
        /// <summary>
        /// Returns the ImageSource by the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ImageSource GetImageSource(string path)
        {
            return ImageSource.FromStream(() =>
            {
                using (Stream stream = File.OpenRead(path))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    return new MemoryStream(bytes);
                }
            });
        }
        #endregion

        #region CreateThumbnailAndReturnThumbnailPath
        /// <summary>
        /// Creates the Thumbnails-folder if it doesn't already exist.
        /// After that a downscaled thumbnail will be created and it's path will be returned.
        /// If these processes fail, an exception will be thrown.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filePath"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static string CreateThumbnailAndReturnThumbnailPath(string filename, string filePath, int quality)
        {
            try
            {
                string thumbnailDirectoryPath = CreateDirectory("Thumbnails");
                string thumbnailPath = Path.Combine(thumbnailDirectoryPath, filename);

                SaveThumbnail(filePath, thumbnailPath, quality);

                return thumbnailPath;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SaveThumbnail
        /// <summary>
        /// Creates thumbnail by using the filepath of the created file to create a bitmap.
        /// This bitmap will be encoded to a jpeg with the given quality compared to the original.
        /// In the end, it will be saved under the thumbnails-folder.
        /// If the process fails, an exception will be thrown and the already saved original file will be deleted.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="quality"></param>
        private static void SaveThumbnail(string filePath, string thumbnailPath, int quality)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);

                    SKBitmap resourceBitmap = SKBitmap.Decode(bytes);

                    Stream thumbnailStream = resourceBitmap.Encode(SKEncodedImageFormat.Jpeg, quality).AsStream();

                    SaveFile(thumbnailStream, thumbnailPath);
                }
            } catch(Exception exc)
            {
                File.Delete(filePath);
                throw exc;
            }
        }
        #endregion

        #region IsFiletypeValid
        /// <summary>
        /// Checks if filetype is valid (jpg, jpeg, mp3, mp4, html, txt)
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsFiletypeValid(string fileType)
        {
            return fileType.Contains("jpg") || 
                fileType.Contains("jpeg") || 
                fileType.Contains("png") || 
                fileType.Contains("mp3") || 
                fileType.Contains("mp4") || 
                fileType.Contains("html") || 
                fileType.Contains("txt");
        }
        #endregion

    }

}
