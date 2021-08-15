using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace WpfPlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : Page, INotifyPropertyChanged
	{
		private Model model;
		private bool isInputImageChanged;
		private readonly Stopwatch stopwatch = new Stopwatch();
		private readonly int fps = 24;

		public double MinR { get; set; } = 0;
		public double MaxR { get; set; } = 254;
		public double MinG { get; set; } = 0;
		public double MaxG { get; set; } = 254;
		public double MinB { get; set; } = 0;
		public double MaxB { get; set; } = 254;
		public ImageSource ImageSource => Image?.ToBitmapSource();

		public Image<Rgba, byte> Image { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public FilterPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += FilterPage_Loaded;
			Unloaded += FilterPage_Unloaded;
			PropertyChanged += FilterPage_PropertyChanged;
		}
		public FilterPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.Filter)) {
				MinR = model.Filter.Min.R;
				MinG = model.Filter.Min.G;
				MinB = model.Filter.Min.B;
				MaxR = model.Filter.Max.R;
				MaxG = model.Filter.Max.G;
				MaxB = model.Filter.Max.B;
			}
			else if (e.PropertyName == nameof(model.CroppedImage)) {
				FilterImage();
				isInputImageChanged = true;
			}
		}

		private void FilterPage_Loaded(object sender, RoutedEventArgs e)
		{
			
		}
		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.Filter = (Color.FromRgb(
				(byte)Math.Round(MinR),
				(byte)Math.Round(MinG),
				(byte)Math.Round(MinB)),
				Color.FromRgb(
				(byte)Math.Round(MaxR),
				(byte)Math.Round(MaxG),
				(byte)Math.Round(MaxB)));
			if (isInputImageChanged) {
				model.FilteredImage = Image;
			}
#if DEBUG
			Debug.WriteLine("MinR: " + MinR);
			Debug.WriteLine("MinG: " + MaxR);
			Debug.WriteLine("MinB: " + MinG);
			Debug.WriteLine("MaxR: " + MaxG);
			Debug.WriteLine("MaxG: " + MinB);
			Debug.WriteLine("MaxB: " + MaxB);
#endif
		}

		private void FilterPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!IsLoaded) {
				return;
			}
			if ((nameof(MinR) + nameof(MaxR) +
				nameof(MinG) + nameof(MaxG) +
				nameof(MinB) + nameof(MaxB)).Contains(e.PropertyName)) {
				FilterImage();
			}
		}

		private void FilterImage()
		{
			var min = Color.FromArgb(255,
				(byte)Math.Round(MinR),
				(byte)Math.Round(MinG),
				(byte)Math.Round(MinB));
			var max = Color.FromArgb(255,
				(byte)Math.Round(MaxR),
				(byte)Math.Round(MaxG),
				(byte)Math.Round(MaxB));
			Image = Methods.FilterRGB(model.CroppedImage, min, max);
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
