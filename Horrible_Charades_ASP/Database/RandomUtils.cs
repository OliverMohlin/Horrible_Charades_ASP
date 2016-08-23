using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horrible_Charades_ASP.Database //Todo: när vi byter namn på mapp byt namespace
{
    public static class RandomUtils
    {
        public static Random rnd = new Random();

        public static int ReturnValue(int upperBound)
        {
            return ReturnValue(0, upperBound);

        }
        public static int ReturnValue(int lower, int upperBound)
        {
            return rnd.Next(lower, upperBound);
        }
    }
}