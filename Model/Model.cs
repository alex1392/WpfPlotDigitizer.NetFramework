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

		public Model()
		{
			PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
				case nameof(InputBitmapImage):
					UpdateInputImage();
					break;
				case nameof(AxisLocation):
					UpdateCroppedImage();
					break;
				case nameof(Filter):
					UpdateFilteredImage();
					break;
				case nameof(FilteredImage):
					// update edittedImage immidiately after setting filteredImage as there is no update trigger in editpage
					UpdateEdittedImage();
					break;
				default:
					break;
			}
		}

		private void UpdateEdittedImage()
		{
			EdittedImage = FilteredImage.Copy();
		}

		private void UpdateFilteredImage()
		{
			FilteredImage = Methods.FilterRGB(CroppedImage, Filter.Min, Filter.Max);
		}

		private void UpdateCroppedImage()
		{
			CroppedImage = Methods.CropImage(InputImage, new Rectangle(
									(int)Math.Round(AxisLocation.X),
									(int)Math.Round(AxisLocation.Y),
									(int)Math.Round(AxisLocation.Width),
									(int)Math.Round(AxisLocation.Height)));
		}

		private void UpdateInputImage()
		{
			InputImage = InputBitmapImage.ToBitmap().ToImage<Rgba, byte>();
		}
	}

	public enum DataType
	{
		Continuous,
		Discrete,
	}

}
