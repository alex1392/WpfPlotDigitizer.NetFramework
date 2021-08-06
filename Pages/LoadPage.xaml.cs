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
		public LoadPage()
		{
			InitializeComponent();
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
			setImage(image);
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
			setImage(image);
		}

		private void setImage(BitmapImage image)
		{
			Model.InputImage = image;
			this.image.Source = image;
			// next page
		}
		private void pasteImage()
		{
			if (Clipboard.ContainsImage())
			{
				var source = Clipboard.GetImage();
				var image = source2image(source);
				setImage(image);
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
				setImage(image);
			}
			else
			{
				MessageBox.Show("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}
		private static BitmapImage source2image(BitmapSource source)
		{
			var encoder = new JpegBitmapEncoder();
			var memoryStream = new MemoryStream();
			var image = new BitmapImage();

			encoder.Frames.Add(BitmapFrame.Create(source));
			encoder.Save(memoryStream);

			memoryStream.Position = 0;
			image.BeginInit();
			image.StreamSource = memoryStream;
			image.EndInit();

			memoryStream.Close();

			return image;
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
