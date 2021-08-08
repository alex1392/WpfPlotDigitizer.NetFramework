using GalaSoft.MvvmLight.Command;
using PropertyChanged;
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
using Rectangle = System.Drawing.Rectangle;

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for AxisPage.xaml
	/// </summary>
	public partial class AxisPage : Page, INotifyPropertyChanged
	{
		private AppData data;

		public AxisPage()
		{
			InitializeComponent();
			DataContext = this;
			GetAxisCommand = new RelayCommand(GetAxis);
			Loaded += AxisPage_Loaded;
			Unloaded += AxisPage_Unloaded;
		}

		private void AxisPage_Loaded(object sender, RoutedEventArgs e)
		{
			ImageSource = data.InputBitmapImage;
		}

		private void AxisPage_Unloaded(object sender, RoutedEventArgs e)
		{
			var axisLocation = new Rectangle((int)AxisLeft, (int)AxisTop, (int)AxisWidth, (int)AxisHeight);
			data.CroppedImage = Methods.CropImage(data.InputImage, axisLocation);
		}

		public AxisPage(AppData data) : this()
		{
			this.data = data;
			data.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(data.InputImage))
				{
					GetAxis();
				}
			};
		}

		public ImageSource ImageSource { get; private set; }

		public double AxisLeft { get; set; }

		public double AxisTop { get; set; }

		public double AxisWidth { get; set; }

		public double AxisHeight { get; set; }

		public ICommand GetAxisCommand { get; set; }

		private void GetAxis()
		{
			var image = data.InputImage;
			var axis = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			AxisLeft = axis.Left;
			AxisTop = axis.Top;
			AxisWidth = axis.Width;
			AxisHeight = axis.Height;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
