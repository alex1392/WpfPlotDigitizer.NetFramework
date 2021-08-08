using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : Page, INotifyPropertyChanged
	{
		private AppData data;
		private Image<Rgba, byte> filteredImage;

		public FilterPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += FilterPage_Loaded;
			Unloaded += FilterPage_Unloaded;
		}

		private void FilterPage_Loaded(object sender, RoutedEventArgs e)
		{
			FilterImage();
		}
		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			data.FilteredImage = filteredImage;
		}

		private void FilterImage()
		{
			var min = Color.FromArgb(255,
				(byte)RangeSliderRed.RangeStartSelected,
				(byte)RangeSliderGreen.RangeStartSelected,
				(byte)RangeSliderBlue.RangeStartSelected);
			var max = Color.FromArgb(255,
				(byte)RangeSliderRed.RangeStopSelected,
				(byte)RangeSliderGreen.RangeStopSelected,
				(byte)RangeSliderBlue.RangeStopSelected);
			filteredImage = Methods.FilterRGB(data.CroppedImage, min, max);
			ImageSource = filteredImage.ToBitmapSource();
		}

		public FilterPage(AppData data) : this()
		{
			this.data = data;
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ImageSource ImageSource { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void RangeSlider_RangeSelectionChanged(object sender, RangeSelectionChangedEventArgs e)
		{
			if (!IsLoaded)
			{
				return;
			}
			FilterImage();
		}
	}
}
