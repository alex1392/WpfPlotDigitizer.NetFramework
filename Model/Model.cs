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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageViewer = Emgu.CV.UI.ImageViewer;
using System.Windows.Interop;
using PropertyChanged;
using System.Windows.Shapes;

namespace WpfPlotDigitizer.NetFramework
{
	public class Model : INotifyPropertyChanged
	{
		public Model()
		{
			PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
				case nameof(InputBitmapImage):
					InputImage = InputBitmapImage.ToBitmap().ToImage<Rgba, byte>();
					break;
				case nameof(AxisLocation):
					CroppedImage = Methods.CropImage(InputImage, new Rectangle(
						(int)Math.Round(AxisLocation.X),
						(int)Math.Round(AxisLocation.Y),
						(int)Math.Round(AxisLocation.Width),
						(int)Math.Round(AxisLocation.Height)));
					break;
				case nameof(Filter):
					FilteredImage = Methods.FilterRGB(CroppedImage, Filter.Min, Filter.Max);
					break;
				case nameof(FilteredImage):
					// update edittedImage immidiately after setting filteredImage as there is no update trigger in editpage
					EdittedImage = FilteredImage.Copy();
					break;
				default:
					break;
			}
		}

		public BitmapImage InputBitmapImage { get; set; }

		public Image<Rgba, byte> InputImage { get; set; }

		public Rect AxisLimit { get; set; }

		public Point AxisLogBase { get; set; }

		public Rect AxisLocation { get; set; }

		public Image<Rgba, byte> CroppedImage { get; set; }

		public (Color Min, Color Max) Filter { get; set; }

		public Image<Rgba, byte> FilteredImage { get; set; }
		public Image<Rgba, byte> EdittedImage { get; set; }

		public DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public enum DataType
	{
		Continuous,
		Discrete,
	}

	public static class Methods
	{
		public static Rectangle? GetAxisLocation(Image<Rgba, byte> image)
		{
			var gray = new Mat();
			CvInvoke.CvtColor(image, gray, ColorConversion.Rgba2Gray);

			var binary = new Mat();
			var threshold = CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.Otsu | ThresholdType.BinaryInv);

			var rectangles = new List<Rectangle>();
			using (var contours = new VectorOfVectorOfPoint()) {
				CvInvoke.FindContours(binary, contours, null, RetrType.List,
					ChainApproxMethod.ChainApproxSimple);
				int count = contours.Size;
				for (int i = 0; i < count; i++) {
					using (var contour = contours[i])
					using (var approxContour = new VectorOfPoint()) {
						CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, false) * 0.01, false);
						rectangles.Add(CvInvoke.BoundingRectangle(approxContour));
					}
				}
			}
#if DEBUG
			var rectanglesImage = new Mat(image.Size, DepthType.Cv8U, 3);
			foreach (var rectangle in rectangles) {
				CvInvoke.Rectangle(rectanglesImage, rectangle, new Bgr(System.Drawing.Color.DarkOrange).MCvScalar);
			}
			//ImageViewer.Show(rectanglesImage);
#endif

			var filtered = rectangles.Where(r =>
				r.Width * r.Height > image.Width * image.Height * 0.25 &&
				r.Width * r.Height < image.Width * image.Height * 0.9);
			if (!filtered.Any()) {
				return null;
			}
			var maxArea = filtered.Max(r => r.Width * r.Height);
			var axis = filtered.First(r => r.Width * r.Height == maxArea);
			return axis;
		}

		public static Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi)
		{
			return image.ToBitmap().ToImage<Rgba, byte>().Copy(roi);
		}

		public static Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Color min, Color max)
		{
			var mask = image.InRange(new Rgba(min.R, min.G, min.B, min.A), new Rgba(max.R, max.G, max.B, max.A));
			image = image.Copy();
			image.SetValue(new Rgba(0, 0, 0, 0), mask.Not());
			return image;
		}

		public static void EraseImage(Image<Rgba, byte> image, Rectangle rect)
		{
			CvInvoke.Rectangle(image, rect, new Rgba().MCvScalar, -1);
		}
		public static List<Point> GetContinuousPoints(Image<Rgba, byte> image)
		{
			var points = new List<Point>();
			var width = image.Width;
			var height = image.Height;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (image.Data[y, x, 3] == 0) {
						continue;
					}
					points.Add(new Point(x, y));

					CvInvoke.DrawMarker(image, new System.Drawing.Point(x, y), new Rgba(255, 0, 0, 255).MCvScalar, MarkerTypes.Cross, 1);
				}
			}

			return points;
		}

		public static List<Point> GetDiscretePoints(Image<Rgba, byte> image)
		{
			var points = new List<Point>();
			var binary = image.InRange(new Rgba(0, 0, 0, 1), new Rgba(255, 255, 255, 255));
			using (var contours = new VectorOfVectorOfPoint()) {
				CvInvoke.FindContours(binary, contours, null, RetrType.List,
					ChainApproxMethod.ChainApproxSimple);
				int count = contours.Size;
				for (int i = 0; i < count; i++) {
					using (var contour = contours[i]) {
						var moments = CvInvoke.Moments(contour);
						var Cx = (int)Math.Round(moments.M10 / moments.M00);
						var Cy = (int)Math.Round(moments.M01 / moments.M00);
						points.Add(new Point(Cx, Cy));

						CvInvoke.DrawMarker(image, new System.Drawing.Point(Cx, Cy), new Rgba(255, 0, 0, 255).MCvScalar, MarkerTypes.Cross, 5);
					}
				}
			}
			return points;
		}

		public static List<Point> TransformData(List<Point> points, Size imageSize, Rect axLim, Point axLogBase)
		{
			var dataList = points.Select(pos =>
			{
				var data = new Point
				{
					X = ModelHelper.LinConvert(pos.X, imageSize.Width, 0, axLim.Right, axLim.Left),
					Y = ModelHelper.LinConvert(imageSize.Height - pos.Y, imageSize.Height, 0, axLim.Bottom, axLim.Top),
				};

				if (axLogBase.X > 0)
					data.X = (float)Math.Pow(
					  axLogBase.X,
					  ModelHelper.LinConvert(data.X, axLim.Left, axLim.Right,
						ModelHelper.LogBase(axLogBase.X, axLim.Left),
						ModelHelper.LogBase(axLogBase.X, axLim.Right))
					  );
				if (axLogBase.Y > 0)
					data.Y = (float)Math.Pow(
					  axLogBase.Y,
					  ModelHelper.LinConvert(data.Y, axLim.Top, axLim.Bottom,
						ModelHelper.LogBase(axLogBase.Y, axLim.Top),
						ModelHelper.LogBase(axLogBase.Y, axLim.Bottom))
					  );
				return data;
			}).ToList();

			return dataList;
		}

		public static void EraseImage(Image<Rgba, byte> image, Polyline polyline)
		{
			// erase all pixels within polyline
			var pointsChecked = new HashSet<Point>();
			var boundary = polyline.RenderedGeometry.Bounds;
			var xmin = (int)Math.Round(boundary.Left);
			var xmax = (int)Math.Round(boundary.Right);
			var ymin = (int)Math.Round(boundary.Top);
			var ymax = (int)Math.Round(boundary.Bottom);
			for (var x = xmin; x < xmax; x++) {
				for (var y = ymin; y < ymax; y++) {
					FloodFill(x, y);
				}
			}

			void FloodFill(int x, int y)
			{
				var point = new Point(x, y);
				if (pointsChecked.Contains(point)) {
					return;
				}
				pointsChecked.Add(point);
				if (!polyline.RenderedGeometry.FillContains(point)) {
					return;
				}
				// erase pixel (set to transparent)
				image.Data[y, x, 3] = 0;
				// recursion
				FloodFill(x + 1, y);
				FloodFill(x, y + 1);
				FloodFill(x - 1, y);
				FloodFill(x, y - 1);
			}
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
			using (var stream = new MemoryStream()) {
				var enc = new PngBitmapEncoder(); // 使用PngEncoder才不會流失透明度
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(stream);
				return new Bitmap(stream);
			}
		}

		public static BitmapImage ToBitmapImage(this BitmapSource source)
		{
			using (var stream = new MemoryStream()) {
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

	public static class ModelHelper
	{
		public static double LinConvert(double value1, double max1, double min1, double max2, double min2)
		{
			var r = (max2 - min2) / (max1 - min1);
			return (min2 + (value1 - min1) * r);
		}
		public static double LogBase(double Base, double num)
		{
			return Math.Log(num) / Math.Log(Base);
		}
	}
}
