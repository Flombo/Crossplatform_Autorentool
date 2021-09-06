﻿using System;
using System.IO;

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
        /// <returns></returns>
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

    }

}
