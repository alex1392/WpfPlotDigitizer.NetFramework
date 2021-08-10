using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for PreviewPage.xaml
	/// </summary>
	public partial class PreviewPage : Page, INotifyPropertyChanged
	{
		private Model model;
		private List<Point> data;

		public event PropertyChangedEventHandler PropertyChanged;

		public Image<Rgba, byte> Image { get; private set; }

		public ImageSource ImageSource => Image?.ToBitmapSource();

		public RelayCommand ExportCommand { get; private set; }

		private PreviewPage()
		{
			InitializeComponent();
			DataContext = this;
			ExportCommand = new RelayCommand(Export, CanExport);
			Loaded += PreviewPage_Loaded;
			Unloaded += PreviewPage_Unloaded;
		}


		public PreviewPage(Model model) : this()
		{
			this.model = model;
		}
		private void PreviewPage_Loaded(object sender, RoutedEventArgs e)
		{
			Image = model.EdittedImage;
			continuousButton.IsChecked = true;
		}
		private void PreviewPage_Unloaded(object sender, RoutedEventArgs e)
		{
			discreteButton.IsChecked = false;
			continuousButton.IsChecked = false;
		}

		private void discreteButton_Checked(object sender, RoutedEventArgs e)
		{
			Image = model.EdittedImage.Copy();
			var points = Methods.GetDiscretePoints(Image);
			OnPropertyChanged(nameof(ImageSource));

			data = Methods.TransformData(points, new Size(Image.Width, Image.Height), model.AxisLimit, model.AxisLogBase);
		}

		private void continuousButton_Checked(object sender, RoutedEventArgs e)
		{
			Image = model.EdittedImage.Copy();
			var points = Methods.GetContinuousPoints(Image);
			OnPropertyChanged(nameof(ImageSource));
			data = Methods.TransformData(points, new Size(Image.Width, Image.Height), model.AxisLimit, model.AxisLogBase);
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Export()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel (.xlsx) | *.xlsx |CSV (.csv) | *.csv |TXT (.txt) | *.txt";
			if (saveFileDialog.ShowDialog() == false)
				return;
			
			TrySave(saveFileDialog.FilterIndex);

			

			bool SaveAsExcel()
			{
				int dataCount = data.Count;
				object[,] dataArray = new object[data.Count + 1, 2];
				dataArray[0, 0] = "X";
				dataArray[0, 1] = "Y";
				for (int i = 0; i < dataCount; i++) {
					dataArray[i + 1, 0] = data[i].X;
					dataArray[i + 1, 1] = data[i].Y;
				}

				var excel = new Excel.Application()
				{
					Visible = false,
					DisplayAlerts = false,
				};
				var wBook = excel.Workbooks.Add(Type.Missing);
				var wSheet = (Excel._Worksheet)wBook.Worksheets[1];
				try {
					wBook.Activate();
					wSheet.Activate();

					string finalColLetter = "B";
					string excelRange = string.Format("A1:{0}{1}",
						finalColLetter, dataCount + 1);

					wSheet.get_Range(excelRange, Type.Missing).Value2 = dataArray;
					wBook.SaveAs(saveFileDialog.FileName);
					wBook.Close(false);
					excel.Quit();
					return true;
				}
				catch (Exception) {
					return false;
				}
				finally {
					Marshal.ReleaseComObject(wSheet);
					Marshal.ReleaseComObject(excel);
					wSheet = null;
					excel = null;
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
			bool SaveText(string seperator)
			{
				try {
					string strPath = saveFileDialog.FileName;

					StringBuilder content = new StringBuilder();
					content.AppendLine("X" + seperator + "Y");
					int dataCount = data.Count;
					for (int i = 0; i < dataCount; i++) {
						content.AppendLine(data[i].X.ToString() + seperator + data[i].Y.ToString());
					}

					using (var fs = File.OpenWrite(strPath))
					using (var sw = new StreamWriter(fs)) {
						sw.Write(content.ToString());
					}

					return true;
				}
				catch (Exception) {
					return false;
				}
			}
			bool SaveAsCSV()
			{
				return SaveText(",");
			}
			bool SaveAsTXT()
			{
				return SaveText("\t");
			}

			void TrySave(int index)
			{
				var IsSucessful = false;

				switch (index) {
					case 1:
					default:
						IsSucessful = SaveAsExcel();
						break;
					case 2:
						IsSucessful = SaveAsCSV();
						break;
					case 3:
						IsSucessful = SaveAsTXT();
						break;
				}

				if (IsSucessful) {
					MessageBox.Show("The data has been exported successfully.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else {
					var result = MessageBox.Show("Something went wrong... try again?", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel);
					if (result == MessageBoxResult.OK) {
						TrySave(index);
					}
				}
			}
		}
		private bool CanExport()
		{
			return true;
		}
	}

}
