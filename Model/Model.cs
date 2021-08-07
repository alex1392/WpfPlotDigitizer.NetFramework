using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace WpfPlotDigitizer2
{
	public class Model : INotifyPropertyChanged
	{
		public BitmapImage InputImage { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public Rect? GetAxis(BitmapImage bitmapImage)
		{
			var bitmap = bitmapImage.ToBitmap();
			var mat = bitmap.ToMat();

			var gray = new Mat();
			CvInvoke.CvtColor(mat, gray, ColorConversion.Rgba2Gray);

			var binary = new Mat();
			var threshold = CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.Otsu | ThresholdType.BinaryInv);

			var axis = new Rect();
			var rectangles = new List<Rectangle>();
			using (var contours = new VectorOfVectorOfPoint())
			{
				CvInvoke.FindContours(binary, contours, null, RetrType.List,
					ChainApproxMethod.ChainApproxSimple);
				int count = contours.Size;
				for (int i = 0; i < count; i++)
				{
					using (var contour = contours[i])
					using (var approxContour = new VectorOfPoint())
					{
						CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, false) * 0.01, false);
						rectangles.Add(CvInvoke.BoundingRectangle(approxContour));
					}
				}
			}
#if DEBUG
			var rectanglesImage = new Mat(mat.Size, DepthType.Cv8U, 3);
			foreach (var rectangle in rectangles)
			{
				CvInvoke.Rectangle(rectanglesImage, rectangle, new Bgr(Color.DarkOrange).MCvScalar);
			}
			ImageViewer.Show(rectanglesImage);
#endif

			var filtered = rectangles.Where(r => 
				r.Width * r.Height > mat.Width * mat.Height * 0.25 && 
				r.Width * r.Height < mat.Width * mat.Height * 0.9);
			if (!filtered.Any())
			{
				return null;
			}
			var maxArea = filtered.Max(r => r.Width * r.Height);
			var axisRect = filtered.First(r => r.Width * r.Height == maxArea);
			axis = new Rect(axisRect.X, axisRect.Y, axisRect.Width, axisRect.Height);
			return axis;
		}

		private bool IsRectangle(VectorOfPoint approxContour)
		{
			if (approxContour.Size != 4)
			{
				return false;
			}
			// determine if all the angles in the contour are within [80, 100] degree
			bool isRectangle = true;
			var pts = approxContour.ToArray();
			var edges = PointCollection.PolyLine(pts, true);

			for (int j = 0; j < edges.Length; j++)
			{
				double angle = System.Math.Abs(
					edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
				if (angle < 80 || angle > 100)
				{
					return false;
				}
			}
			return true;
		}
	}

	public static class ImageConverter
	{
		/// <summary>
		/// Delete a GDI object
		/// </summary>
		/// <param name="o">The poniter to the GDI object to be deleted</param>
		/// <returns></returns>
		[DllImport("gdi32")]
		private static extern int DeleteObject(IntPtr o);

		/// <summary>
		/// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
		/// </summary>
		/// <param name="image">The Emgu CV Image</param>
		/// <returns>The equivalent BitmapSource</returns>
		public static BitmapSource ToBitmapSource<TColor, TDepth>(Image<TColor, TDepth> image)
			where TColor : struct, IColor
			where TDepth : new()
		{
			using (var source = image.ToBitmap())
			{
				IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

				BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					ptr,
					IntPtr.Zero,
					Int32Rect.Empty,
					System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

				DeleteObject(ptr); //release the HBitmap
				return bs;
			}
		}

		public static System.Drawing.Bitmap ToBitmap(this BitmapSource source)
		{
			using (var stream = new MemoryStream())
			{
				var enc = new PngBitmapEncoder(); // 使用PngEncoder才不會流失透明度
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(stream);
				return new System.Drawing.Bitmap(stream);
			}
		}

		public static BitmapImage ToBitmapImage(this BitmapSource source)
		{
			using (var stream = new MemoryStream())
			{
				//var encoder = new JpegBitmapEncoder();
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(stream);

				var image = new BitmapImage();
				stream.Position = 0;
				image.BeginInit();
				image.StreamSource = stream;
				image.EndInit();
				return image;
			}
		}
	}
}
