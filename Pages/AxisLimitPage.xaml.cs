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
		private Model model;

		public AxisLimitPage()
		{
			InitializeComponent();
			DataContext = this;
			this.Unloaded += AxisLimitPage_Unloaded;
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(YMax, out var ymax) ||
				!double.TryParse(YMin, out var ymin) ||
				!double.TryParse(XMax, out var xmax) ||
				!double.TryParse(XMin, out var xmin))
			{
				MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			model.AxisLimit = new Rect(new Point(xmin, ymin), new Point(xmax, ymax));
			double ylog = 0, xlog = 0;
			if (!string.IsNullOrEmpty(YLog) &&
				!double.TryParse(YLog, out ylog) ||
				!string.IsNullOrEmpty(XLog) &&
				!double.TryParse(XLog, out xlog))
			{
				MessageBox.Show("Cannot parse axis log base, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			model.YLogBase = ylog;
			model.XLogBase = xlog;
		}

		public AxisLimitPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += (sender, e) =>
			{
				OnPropertyChanged(nameof(ImageSource));
			};
		}

		public ImageSource ImageSource => model.InputImage;

		public string YMax { get; set; }
		public string YLog { get; set; }
		public string YMin { get; set; }
		public string XMax { get; set; }
		public string XLog { get; set; }
		public string XMin { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
