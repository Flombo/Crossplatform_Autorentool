using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Autorentool_RMT.Models
{
    public class Model
    {

        #region Model ToString-method
        /// <summary>
        /// returns propertyInfos as an appended string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            Type objType = GetType();
            PropertyInfo[] propertyInfoList = objType.GetProperties();
            StringBuilder result = new StringBuilder();

            foreach (PropertyInfo propertyInfo in propertyInfoList)
                result.AppendFormat("{0}={1} ", propertyInfo.Name, propertyInfo.GetValue(this));

            return result.ToString();
        }
        #endregion

        #region GetImageSource
        protected ImageSource GetImageSource(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] bArray = new byte[stream.Length];
                stream.Read(bArray, 0, (int)stream.Length);
                int length = bArray.Length;


                return ImageSource.FromStream(() => new MemoryStream(bArray));
            }
        }
        #endregion

    }
}
