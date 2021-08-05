using System;
using System.Reflection;
using System.Text;

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

    }
}
