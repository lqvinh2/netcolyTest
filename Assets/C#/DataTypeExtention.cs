using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.C_
{
    public static class DataTypeExtention
    {
        public static string Str(this object obj)
        {
            if ((obj is DBNull) || (obj == null))
            {
                return string.Empty;
            }
            return Convert.ToString(obj);
        }


    }
}
