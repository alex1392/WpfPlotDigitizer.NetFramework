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
using System.Threading;
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

namespace PlotDigitizer.NetFramework
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

		public bool IsDiscrete { get; set; }

		public bool IsContinuous { get; set; } = true;

		private PreviewPage()
		{
			InitializeComponent();
			DataContext = this;
			ExportCommand = new RelayCommand(Export, CanExport);
			Loaded += PreviewPage_Loaded;
			Unloaded += PreviewPage_Unloaded;
			PropertyChanged += PreviewPage_PropertyChanged;
		}


		public PreviewPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.DataType)) {
				IsDiscrete = model.DataType == DataType.Discrete;
				IsContinuous = model.DataType == DataType.Continuous;
			}
		}

		private void PreviewPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (IsDiscrete) {
				OnPropertyChanged(nameof(IsDiscrete));
			}
			else if (IsContinuous) {
				OnPropertyChanged(nameof(IsContinuous));
			}
		}
		private void PreviewPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.DataType = IsDiscrete ? DataType.Discrete : DataType.Continuous;
		}

		private void PreviewPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IsDiscrete) && IsDiscrete) {
				Image = model.EdittedImage.Copy();
				var points = Methods.GetDiscretePoints(Image);
				OnPropertyChanged(nameof(ImageSource));
				data = Methods.TransformData(points, new Size(Image.Width, Image.Height), model.AxisLimit, model.AxisLogBase);

			}
			else if (e.PropertyName == nameof(IsContinuous) && IsContinuous) {
				Image = model.EdittedImage.Copy();
				var points = Methods.GetContinuousPoints(Image);
				OnPropertyChanged(nameof(ImageSource));
				data = Methods.TransformData(points, new Size(Image.Width, Image.Height), model.AxisLimit, model.AxisLogBase);

			}
		}


		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Export()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel |*.xlsx|" +
				"CSV |*.csv|" + 
				"TXT |*.txt";
			if (saveFileDialog.ShowDialog() == false)
				return;

			TrySave(saveFileDialog.FilterIndex);

			ExportResults SaveAsExcel(CancellationToken token)
			{
				int dataCount = data.Count;
				object[,] dataArray = new object[data.Count + 1, 2];
				dataArray[0, 0] = "X";
				dataArray[0, 1] = "Y";
				for (int i = 0; i < dataCount; i++) {
					dataArray[i + 1, 0] = data[i].X;
					dataArray[i + 1, 1] = data[i].Y;
					if (token.IsCancellationRequested) {
						return ExportResults.Canceled;
					}
				}

				if (token.IsCancellationRequested) {
					return ExportResults.Canceled;
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
					if (token.IsCancellationRequested) {
						return ExportResults.Canceled;
					}
					wBook.SaveAs(saveFileDialog.FileName);
					wBook.Close(false);
					excel.Quit();
					return ExportResults.Sucessful;
				}
				catch (Exception) {
					return ExportResults.Failed;
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
			ExportResults SaveAsCSV(CancellationToken token)
			{
				return SaveText(",", token);
			}
			ExportResults SaveAsTXT(CancellationToken token)
			{
				return SaveText("\t", token);
			}
			ExportResults SaveText(string seperator, CancellationToken token)
			{
				try {
					string strPath = saveFileDialog.FileName;

					StringBuilder content = new StringBuilder();
					content.AppendLine("X" + seperator + "Y");
					int dataCount = data.Count;
					for (int i = 0; i < dataCount; i++) {
						content.AppendLine(data[i].X.ToString() + seperator + data[i].Y.ToString());
						if (token.IsCancellationRequested) {
							return ExportResults.Canceled;
						}
					}

					using (var fs = File.OpenWrite(strPath))
					using (var sw = new StreamWriter(fs)) {
						sw.Write(content.ToString());
					}

					return ExportResults.Sucessful;
				}
				catch (Exception) {
					return ExportResults.Failed;
				}
			}

			async void TrySave(int index)
			{
				Mouse.OverrideCursor = Cursors.Wait;

				var cts = new CancellationTokenSource();
				var token = cts.Token;
				var saveTask = Task.Run(() =>
				{
					switch (index) {
						default:
						case 1:
							return SaveAsExcel(token);
						case 2:
							return SaveAsCSV(token);
						case 3:
							return SaveAsTXT(token);
					}
				}, token);
				var popup = new ProgressPopup();
				popup.Owner = Application.Current.MainWindow;
				popup.Canceled += (sender, e) =>
				{
					cts.Cancel();
				};
				popup.Show();

				var result = await saveTask;
				popup.Close();
				Mouse.OverrideCursor = null;

				if (result == ExportResults.Sucessful) {
					MessageBox.Show("The data has been exported successfully.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if (result == ExportResults.Failed) {
					var response = MessageBox.Show("Something went wrong... try again?", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel);
					if (response == MessageBoxResult.OK) {
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

	public enum ExportResults
	{
		None,
		Sucessful,
		Failed,
		Canceled
	}
}
