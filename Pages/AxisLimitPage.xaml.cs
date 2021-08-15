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
	/// Interaction logic for AxisLimitPage.xaml
	/// </summary>
	public partial class AxisLimitPage : Page, INotifyPropertyChanged
	{
		private Model model;

		public AxisLimitPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += AxisLimitPage_Loaded;
			Unloaded += AxisLimitPage_Unloaded;
		}
		public AxisLimitPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void AxisLimitPage_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.AxisLimit = new Rect(new Point(xMin ?? 0, yMin ?? 0), new Point(xMax ?? 0, yMax ?? 0));
			model.AxisLogBase = new Point(xLog ?? 0, yLog ?? 0);
#if DEBUG
			Debug.WriteLine(nameof(model.AxisLimit) + ": " + model.AxisLimit.ToString());
			Debug.WriteLine(nameof(model.AxisLogBase) + ": " + model.AxisLogBase.ToString());
#endif
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputBitmapImage)) {
				ImageSource = model.InputBitmapImage;
			}
			else if (e.PropertyName == nameof(model.AxisLimit)) {
				xMin = model.AxisLimit.Left;
				xMax = model.AxisLimit.Right;
				yMin = model.AxisLimit.Top;
				yMax = model.AxisLimit.Bottom;
			}
			else if (e.PropertyName == nameof(model.AxisLogBase)) {
				xLog = model.AxisLogBase.X;
				yLog = model.AxisLogBase.Y;
			}
		}

		public ImageSource ImageSource { get; private set; }

		private double? yMax = null;
		private double? yLog = null;
		private double? yMin = null;
		private double? xMax = null;
		private double? xLog = null;
		private double? xMin = null;

		// These properties need to share the same name with the Model's properties
		public string AxisYMax
		{
			get => yMax.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yMax = null;
				}
				else if (double.TryParse(value, out var result)) {
					yMax = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		public string AxisYLog
		{
			get => yLog.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yLog = null;
				}
				else if (double.TryParse(value, out var result)) {
					yLog = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		public string AxisYMin
		{
			get => yMin.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yMin = null;
				}
				else if (double.TryParse(value, out var result)) {
					yMin = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		public string AxisXMax
		{
			get => xMax.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xMax = null;
				}
				else if (double.TryParse(value, out var result)) {
					xMax = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		public string AxisXLog
		{
			get => xLog.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xLog = null;
				}
				else if (double.TryParse(value, out var result)) {
					xLog = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		public string AxisXMin
		{
			get => xMin.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xMin = null;
				}
				else if (double.TryParse(value, out var result)) {
					xMin = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
