using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkBC.Web.Helpers
{
    public static class IntegerExtensions
    {
        public static string GetWord(this int number)
        {
            return number switch
            {
                1 => "one",
                2 => "two",
                3 => "three",
                4 => "four",
                5 => "five",
                6 => "six",
                7 => "seven",
                8 => "eight",
                9 => "nine",
                _ => number.ToString()
            };
        }
    }
}
