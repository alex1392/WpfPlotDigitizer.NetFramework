using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

using PropertyChanged;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Bitmap = System.Drawing.Bitmap;
using ImageViewer = Emgu.CV.UI.ImageViewer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace PlotDigitizer.NetFramework
{
	public static class ImageConverter
	{
		/// <summary>
		/// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
		/// </summary>
		/// <param name="image">The Emgu CV Image</param>
		/// <returns>The equivalent BitmapSource</returns>
		public static BitmapSource ToBitmapSource<TColor, TDepth>(this Image<TColor, TDepth> image)
			where TColor : struct, IColor
			where TDepth : new()
		{
			// must convert to bgra to retain the transparency
			return image.Convert<Bgra, byte>().ToBitmap().ToBitmapSource();
		}

		public static BitmapSource ToBitmapSource(this Bitmap bitmap)
		{
			var hBitmap = bitmap.GetHbitmap();
			BitmapSource source;

			try {
				source = Imaging.CreateBitmapSourceFromHBitmap(
							 hBitmap,
							 IntPtr.Zero,
							 Int32Rect.Empty,
							 BitmapSizeOptions.FromEmptyOptions());
			}
			finally {
				DeleteObject(hBitmap);
			}

			return source;
		}

		public static Bitmap ToBitmap(this BitmapSource source)
		{
			//using (var stream = new MemoryStream()) {
			var stream = new MemoryStream();	
			var enc = new PngBitmapEncoder(); // 使用PngEncoder才不會流失透明度
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(stream);
				return new Bitmap(stream);
			//}
		}

		public static BitmapImage ToBitmapImage(this BitmapSource source)
		{
			//using (var stream = new MemoryStream()) {
			var stream = new MemoryStream();	
			var encoder = new JpegBitmapEncoder();
				//var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(stream);

				var image = new BitmapImage();
				stream.Position = 0;
				image.BeginInit();
				image.StreamSource = stream;
				image.EndInit();
				return image;
			//}
		}

		/// <summary>
		/// Delete a GDI object
		/// </summary>
		/// <param name="o">The poniter to the GDI object to be deleted</param>
		/// <returns></returns>
		[DllImport("gdi32")]
		private static extern int DeleteObject(IntPtr o);
	}
}