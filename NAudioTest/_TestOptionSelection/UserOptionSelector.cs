using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest._TestOptionSelection
{
    public static class UserOptionSelector
    {
        public static string GetUserSelectedOption(List<string> printOptions, string printHeading, string instructions)
        {
            Console.WriteLine(printHeading);
            Dictionary<int, string> printedOptions = new Dictionary<int, string>();
            int printNum = 1;
            printOptions.ForEach(option => {
                printedOptions.Add(printNum, option);
                Console.WriteLine("    " + printNum + ".) " + option);
                printNum++;
            });
            Console.WriteLine(instructions);
            string selectedOption = printedOptions[Int32.Parse(Console.ReadLine())];
            return selectedOption;
        }
    }
}
