using GalaSoft.MvvmLight.Command;
using PropertyChanged;
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
using Rectangle = System.Drawing.Rectangle;

namespace PlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for AxisPage.xaml
	/// </summary>
	public partial class AxisPage : Page, INotifyPropertyChanged
	{
		private Model model;
		public event PropertyChangedEventHandler PropertyChanged;
		public ImageSource ImageSource { get; private set; }

		// These properties need to share the same name with Model's properties
		public double AxisLeft { get; set; }
		public double AxisTop { get; set; }
		public double AxisWidth { get; set; }
		public double AxisHeight { get; set; }

		public ICommand GetAxisCommand { get; set; }

		public AxisPage()
		{
			InitializeComponent();
			DataContext = this;
			GetAxisCommand = new RelayCommand(GetAxis);
			Unloaded += AxisPage_Unloaded;
		}
		public AxisPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}


		private void AxisPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.AxisLocation = new Rect(AxisLeft, AxisTop, AxisWidth, AxisHeight);
#if DEBUG
			Debug.WriteLine(nameof(model.AxisLocation) + ": " + model.AxisLocation.ToString());
#endif
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputBitmapImage)) {
				ImageSource = model.InputBitmapImage;
				GetAxis();
			}
			else if (e.PropertyName == nameof(model.AxisLocation)) {
				AxisLeft = model.AxisLocation.Left;
				AxisTop = model.AxisLocation.Top;
				AxisWidth = model.AxisLocation.Width;
				AxisHeight = model.AxisLocation.Height;
			}
		}

		private void GetAxis()
		{
			var image = model.InputImage;
			var axis = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			AxisLeft = axis.Left;
			AxisTop = axis.Top;
			AxisWidth = axis.Width;
			AxisHeight = axis.Height;
		}

	}
}
