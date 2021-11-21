using Img2Dsj.Models;
using MoreLinq;

namespace Img2Dsj.Utils;
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
    public static List<List<string>> ToMonocolorPixels(this List<List<string>> initialPixels, Settings settings)
    {
        List<List<string>> monocolorPixels = new();
        foreach (List<string> pixels in initialPixels)
        {
            monocolorPixels.Add(pixels.ConvertAll(x =>
            {
                if (x != null)
                {
                    x = settings.ColorToUse?.Replace("#", "0x") ?? "0x000000";
                }
                return x;
            }));
        }

        return monocolorPixels;
    }
}
