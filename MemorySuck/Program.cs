using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MemorySuck
{
    class Program
    {
        static void Main(string[] args)
        {
            double memoryLimit = double.Parse(args[0]) / 100;
            const long outerSize = 1024;
            const long innerSize = 100 * 1024 * 1024;

            Console.Write("sucking memory until the system is at about {0}% usage ...\r\n", memoryLimit * 100);

            var gbArray = new byte[outerSize][];
            Parallel.For(0, outerSize, (g, loopState) =>
            {
                var array = new byte[innerSize];
                for (int i = 0; i < innerSize; i++)
                {
                    array[i] = 0;
                }
                gbArray[g] = array;
                if (PercentMemoryUsed() >= memoryLimit)
                    loopState.Break();
            });

            Console.Write("memory sucked - hit any key to close and release memory\r\n");
            Console.Read();
        }

        private static double PercentMemoryUsed()
        {
            var wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            var searchResults = new ManagementObjectSearcher(wql).Get();

            var resultEnumerator = searchResults.GetEnumerator();
            resultEnumerator.MoveNext();
            var result = resultEnumerator.Current;

            var percentMemoryFree = (double)(UInt64)result["FreePhysicalMemory"] / (double)(UInt64)result["TotalVisibleMemorySize"];
            var percentMemoryUsed = 1.0 - percentMemoryFree;

            return percentMemoryUsed;
        }

    }
}
