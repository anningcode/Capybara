using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Utils
{
    public static class CommonHelper
    {
        public static int FindSmallestMissingPositive(List<int> values)
        {
            var set = new HashSet<int>(values);
            int missing = 1;
            while (set.Contains(missing))
            {
                missing++;
            }
            return missing;
        }
    }
}
