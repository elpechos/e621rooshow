using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621RooShow.ViewModels
{
    public static class ImageAspectRatio
    {
        public static Tuple<int, int> ResizeFit(Tuple<int, int> originalSize, Tuple<int, int> maxSize)
        {
            var widthRatio = (double)maxSize.Item1 / (double)originalSize.Item1;
            var heightRatio = (double)maxSize.Item2 / (double)originalSize.Item2;
            var minAspectRatio = Math.Min(widthRatio, heightRatio);
            //if (minAspectRatio > 1)
            //    return originalSize;
            return Tuple.Create((int)(originalSize.Item1 * minAspectRatio), (int)(originalSize.Item2 * minAspectRatio));
        }


        public static Tuple<int, int> Center(Tuple<int, int> imageSize, Tuple<int, int> maxSize)
        {
            var position = Tuple.Create(maxSize.Item1 - imageSize.Item1, maxSize.Item2 - imageSize.Item2);
            return Tuple.Create(position.Item1 / 2, position.Item2 / 2);
        }
    }
}