using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectanglePackerWindow.Utilities
{
    public static class Extensions
    {
        public static void Randomise<T>(this List<T> list, Random rand)
        {
            SortedDictionary<int, T> map = new SortedDictionary<int, T>();
            foreach (T item in list)
            {
                int r;
                do
                {
                    r = rand.Next();
                }
                while (map.ContainsKey(r));
                map.Add(r, item);
            }

            list.Clear();
            list.AddRange(map.Values);
        }

        public static List<FileInfo> GetFilteredFiles(this DirectoryInfo directory, string[] extensions)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (string ext in extensions)
            {
                files.AddRange(directory.GetFiles(ext, SearchOption.TopDirectoryOnly));
            }
            return files;
        }

        public static void Save(this Canvas canvas, string filePath)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            using (MemoryStream ms = new MemoryStream())
            {
                pngEncoder.Save(ms);
                File.WriteAllBytes(filePath, ms.ToArray());
            }
        }
    }
}