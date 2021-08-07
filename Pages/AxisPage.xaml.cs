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

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for AxisPage.xaml
	/// </summary>
	public partial class AxisPage : Page, INotifyPropertyChanged
	{
		private Model model;

		public AxisPage()
		{
			InitializeComponent();
			DataContext = this;
			GetAxisCommand = new RelayCommand(GetAxis);
		}

		public AxisPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += (sender, e) =>
			{
				OnPropertyChanged(nameof(ImageSource));
				GetAxis();
			};
		}

		public ImageSource ImageSource => 
			model.InputImage;

		public double AxisLeft { get; set; } 

		public double AxisTop { get; set; }

		public double AxisWidth { get; set; } 

		public double AxisHeight { get; set; }

		public ICommand GetAxisCommand { get; set; }

		private void GetAxis()
		{
			var image = model.InputImage;
			var axis = model.GetAxis(image) ?? new Rect(image.PixelWidth / 4, image.PixelHeight / 4, image.PixelWidth / 2, image.PixelHeight / 2);
			AxisLeft = axis.Left;
			AxisTop = axis.Top;
			AxisWidth = axis.Width;
			AxisHeight = axis.Height;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
