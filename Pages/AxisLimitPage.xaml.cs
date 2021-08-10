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
	/// Interaction logic for AxisLimitPage.xaml
	/// </summary>
	public partial class AxisLimitPage : Page, INotifyPropertyChanged
	{
		private Model data;

		public AxisLimitPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += AxisLimitPage_Loaded;
			Unloaded += AxisLimitPage_Unloaded;
		}

		private void AxisLimitPage_Loaded(object sender, RoutedEventArgs e)
		{
			ImageSource = data.InputBitmapImage;
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			double ymax = 0, ymin = 0, xmax = 0, xmin = 0;
			if (!string.IsNullOrEmpty(YMax) && 
				!double.TryParse(YMax, out ymax) ||
				!string.IsNullOrEmpty(YMin) &&
				!double.TryParse(YMin, out ymin) ||
				!string.IsNullOrEmpty(XMax) &&
				!double.TryParse(XMax, out xmax) ||
				!string.IsNullOrEmpty(XMin) &&
				!double.TryParse(XMin, out xmin))
			{
				MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			data.AxisLimit = new Rect(new Point(xmin, ymin), new Point(xmax, ymax));
			double ylog = 0, xlog = 0;
			if (!string.IsNullOrEmpty(YLog) &&
				!double.TryParse(YLog, out ylog) ||
				!string.IsNullOrEmpty(XLog) &&
				!double.TryParse(XLog, out xlog))
			{
				MessageBox.Show("Cannot parse axis log base, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			data.AxisLogBase = new Point(xlog, ylog);
		}

		public AxisLimitPage(Model data) : this()
		{
			this.data = data;
		}

		public ImageSource ImageSource { get; private set; }

		public string YMax { get; set; }
		public string YLog { get; set; }
		public string YMin { get; set; }
		public string XMax { get; set; }
		public string XLog { get; set; }
		public string XMin { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
