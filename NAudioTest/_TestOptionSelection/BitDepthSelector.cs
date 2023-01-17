using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest._TestOptionSelection
{
    public static class BitDepthSelector
    {
        public static eBitDepth SelectBitDepth()
        {
            Dictionary<int, eBitDepth> printedDepths = new Dictionary<int, eBitDepth>();
            int printNum = 1;
            Enum.GetValues(typeof(eBitDepth)).OfType<eBitDepth>().ToList().ForEach(depth => {
                printedDepths.Add(printNum, depth);
                Console.WriteLine("    " + printNum + ".) " + depth);
                printNum++;
            });
            eBitDepth selectedDepth = printedDepths[Int32.Parse(Console.ReadLine())];
            return selectedDepth;
        }
    }
}
