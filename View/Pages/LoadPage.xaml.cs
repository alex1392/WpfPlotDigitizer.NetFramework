using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

namespace PlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for LoadPage.xaml
	/// </summary>
	public partial class LoadPage : Page, INotifyPropertyChanged
	{
		private Model model;

		public event PropertyChangedEventHandler PropertyChanged;

		public BitmapImage Image { get; set; }

		public ImageSource ImageSource => Image;

		public LoadPage()
		{
			InitializeComponent();
			Loaded += LoadPage_Loaded;
			Unloaded += LoadPage_Unloaded;
			PropertyChanged += LoadPage_PropertyChanged;
		}

		public LoadPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void LoadPage_Loaded(object sender, RoutedEventArgs e)
		{
#if DEBUG
			imageControl.Visibility = Visibility.Visible;
#endif
		}

		private void LoadPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.InputBitmapImage = Image;
		}

		private void LoadPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputBitmapImage)) {
				Image = model.InputBitmapImage;
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
			if (dialog.ShowDialog() != true) {
				return;
			}
			var filename = dialog.FileName;
			var image = loadImage(filename);
			if (image is null) {
				return;
			}
			Image = image;
		}

		private void pasteButton_Click(object sender, RoutedEventArgs e)
		{
			pasteImage();
		}

		private void Page_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
				e.Key == Key.V) {
				pasteImage();
			}
		}

		private void Page_DragOver(object sender, DragEventArgs e)
		{
			bool isEnable;
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				var files = (string[])e.Data.GetData(DataFormats.FileDrop);
				var filename = files[0];
				if (File.Exists(filename)) {
					isEnable = true;
				}
				else {
					isEnable = false;
				}
			}
			else {
				isEnable = false;
			}

			if (!isEnable) {
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
			else {
				e.Effects = DragDropEffects.Copy;
			}
		}

		private void Page_Drop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				MessageBox.Show("Input file is not valid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			var filename = files[0];
			var image = loadImage(filename);
			if (image is null) {
				return;
			}
			Image = image;
		}

		private void pasteImage()
		{
			if (Clipboard.ContainsImage()) {
				var source = Clipboard.GetImage();
				var image = source.ToBitmapImage();
				Image = image;
			}
			else if (Clipboard.ContainsFileDropList()) {
				var files = Clipboard.GetFileDropList();
				var file = files[0];
				var image = loadImage(file);
				if (image is null) {
					return;
				}
				Image = image;
			}
			else {
				MessageBox.Show("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		private BitmapImage loadImage(string filename)
		{
			if (!File.Exists(filename)) {
				MessageBox.Show("Input file is not valid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
			try {
				var image = new BitmapImage(new Uri(filename));
				return image;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
		}
	}
}