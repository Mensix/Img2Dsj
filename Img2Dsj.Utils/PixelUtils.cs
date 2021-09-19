using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Img2Dsj.Utils
{
    public static class PixelUtils
    {
        public static List<List<List<string>>> MergeSamePixels(this List<List<string>> initialPixels)
        {
            List<List<List<string>>> mergedPixels = new();
            foreach (List<string> p in initialPixels)
            {
                mergedPixels.Add(p.GroupAdjacent(x => x).Select(x => x.ToList()).ToList());
            }

            return mergedPixels;
        }
    }
}