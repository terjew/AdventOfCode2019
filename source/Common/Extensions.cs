using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Extensions
    {
        public static int GetInt(this Match m, string groupName)
        {
            return int.Parse(m.Groups[groupName].Value);
        }
    }

}
