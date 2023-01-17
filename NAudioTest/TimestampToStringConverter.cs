using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest
{
    public static class TimestampToStringConverter
    {
        public static string TimestampToString(TimeSpan ts)
        {
            return ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s";
        }
    }
}
