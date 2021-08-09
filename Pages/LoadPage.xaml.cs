using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
	/// Interaction logic for LoadPage.xaml
	/// </summary>
	public partial class LoadPage : Page
	{
		private AppData data;

		public LoadPage()
		{
			InitializeComponent();
		}

		public LoadPage(AppData data) : this()
		{
			this.data = data;
		}
		public void SetImage(BitmapImage image)
		{
			data.InputBitmapImage = image;
			data.InputImage = image.ToBitmap().ToImage<Rgba, byte>();
#if DEBUG
			this.image.Source = image;
#endif
			var mainWindow = Application.Current.MainWindow as MainWindow;
			if (mainWindow.PageManager.NextCommand.CanExecute(null))
			{
				mainWindow.PageManager.NextCommand.Execute(null);
			}
		}

		private void browseButton_Loaded(object sender, RoutedEventArgs e)
		{
			(sender as UIElement).Focus();
		}

		private void browseButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "All |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
				"(*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
				"(*.png)|*.png|" +
				"(*.bmp)|*.bmp|" +
				"(*.tif)|*.tif|" +
				"Any |*.*";
			if (dialog.ShowDialog() != true)
			{
				return;
			}
			var filename = dialog.FileName;
			var image = loadImage(filename);
			if (image is null)
			{
				return;
			}
			SetImage(image);
		}

		private void pasteButton_Click(object sender, RoutedEventArgs e)
		{
			pasteImage();
		}

		private void Page_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
				e.Key == Key.V)
			{
				pasteImage();
			}
		}

		private void Page_DragOver(object sender, DragEventArgs e)
		{
			bool isEnable;
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[])e.Data.GetData(DataFormats.FileDrop);
				var filename = files[0];
				if (File.Exists(filename))
				{
					isEnable = true;
				}
				else
				{
					isEnable = false;
				}
			}
			else
			{
				isEnable = false;
			}

			if (!isEnable)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
			else
			{
				e.Effects = DragDropEffects.Copy;
			}
		}

		private void Page_Drop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				MessageBox.Show("Input file is not valid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			var filename = files[0];
			var image = loadImage(filename);
			if (image is null)
			{
				return;
			}
			SetImage(image);
		}
		private void pasteImage()
		{
			if (Clipboard.ContainsImage())
			{
				var source = Clipboard.GetImage();
				var image = source.ToBitmapImage();
				SetImage(image);
			}
			else if (Clipboard.ContainsFileDropList())
			{
				var files = Clipboard.GetFileDropList();
				var file = files[0];
				var image = loadImage(file);
				if (image is null)
				{
					return;
				}
				SetImage(image);
			}
			else
			{
				MessageBox.Show("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}
		
		private BitmapImage loadImage(string filename)
		{
			if (!File.Exists(filename))
			{
				MessageBox.Show("Input file is not valid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
			try
			{
				var image = new BitmapImage(new Uri(filename));
				return image;
			}
			catch (NotSupportedException ex)
			{
				MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
		}

	}
}
