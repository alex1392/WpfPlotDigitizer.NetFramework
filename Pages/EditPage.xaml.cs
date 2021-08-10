using Emgu.CV;
using Emgu.CV.Structure;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for EditPage.xaml
	/// </summary>
	public partial class EditPage : Page, INotifyPropertyChanged
	{
		private Model model;
		private EditState editState;
		private Point mouseDownPos;
		private bool isMoving;
		private readonly double eraserOriginalSize;
		private readonly double eraserOriginalBorderThickness;
		private readonly Stopwatch stopwatch = new Stopwatch();
		private readonly Dictionary<EditState, bool> isEditting = new Dictionary<EditState, bool>
		{
			{ EditState.Eraser, false },
			{ EditState.Rectangle, false },
			{ EditState.Polygon, false },
		};
		private readonly int fps = 24;

		public event PropertyChangedEventHandler PropertyChanged;

		public EditPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;

			eraserOriginalSize = eraserRect.Width;
			eraserOriginalBorderThickness = eraserRect.StrokeThickness;
		}

		public EditPage(Model model) : this()
		{
			this.model = model;
		}
		public EditManager<Image<Rgba, byte>> EditManager { get; private set; }
		public ImageSource ImageSource => Image?.ToBitmapSource();
		public IEnumerable<string> UndoList
		{
			get
			{
				return EditManager?.TagList.GetRange(0, EditManager.Index + 1).Reverse<string>();
			}
		}
		public IEnumerable<string> RedoList
		{
			get
			{
				return EditManager?.TagList.GetRange(EditManager.Index, EditManager.TagList.Count - EditManager.Index);
			}
		}

		public Image<Rgba, byte> Image { get; private set; }


		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			Image = model.FilteredImage;
			EditManager = new EditManager<Image<Rgba, byte>>(Image.Copy());
			EditManager.PropertyChanged += EditManager_PropertyChanged;
		}

		private void EditPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.EdittedImage = Image;
		}
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index)) {
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
				Image = EditManager.CurrentObject.Copy();
			}
		}

		private void EraserButton_Checked(object sender, RoutedEventArgs e)
		{
			RectButton.IsChecked = false;
			PolyButton.IsChecked = false;
			editState = EditState.Eraser;
		}

		private void RectButton_Checked(object sender, RoutedEventArgs e)
		{
			EraserButton.IsChecked = false;
			PolyButton.IsChecked = false;
			editState = EditState.Rectangle;
		}

		private void PolyButton_Checked(object sender, RoutedEventArgs e)
		{
			RectButton.IsChecked = false;
			EraserButton.IsChecked = false;
			editState = EditState.Polygon;
		}

		private void StateButton_Unchecked(object sender, RoutedEventArgs e)
		{
			editState = EditState.None;
			selectRect.Visibility = Visibility.Hidden;
		}

		private void UndoComboBox_DropDownClosed(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox.SelectedIndex == 0)
				return;
			var targetIndex = EditManager.Index - comboBox.SelectedIndex;
			if (EditManager.GoToCommand.CanExecute(targetIndex))
				EditManager.GoToCommand.Execute(targetIndex);
			UndoComboBox.SelectedIndex = 0;
			RedoComboBox.SelectedIndex = 0;
		}

		private void RedoComboBox_DropDownClosed(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox.SelectedIndex == 0)
				return;
			var targetIndex = EditManager.Index + comboBox.SelectedIndex;
			if (EditManager.GoToCommand.CanExecute(targetIndex))
				EditManager.GoToCommand.Execute(targetIndex);
			UndoComboBox.SelectedIndex = 0;
			RedoComboBox.SelectedIndex = 0;
		}

		private void mainGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (editState == EditState.None ||
				e.ChangedButton != MouseButton.Right) {
				return;
			}
			if (editState == EditState.Rectangle) {
				mouseDownPos = e.GetPosition(editCanvas);
				Canvas.SetLeft(selectRect, mouseDownPos.X);
				Canvas.SetTop(selectRect, mouseDownPos.Y);
				selectRect.Width = 0;
				selectRect.Height = 0;
				selectRect.Visibility = Visibility.Visible;
				selectRect.Focus();

				isEditting[editState] = true;
			}
			else if (editState == EditState.Polygon) {

				isEditting[editState] = true;
			}
			else if (editState == EditState.Eraser) {
				eraserRect.Visibility = Visibility.Visible;
				stopwatch.Restart();
				isEditting[editState] = true;
				mainGrid_MouseMove(sender, e);
			}
		}

		private void mainGrid_MouseMove(object sender, MouseEventArgs e)
		{
			if (!isEditting.ContainsKey(editState) ||
				!isEditting[editState] ||
				isMoving)
				return;
			isMoving = true;

			if (editState == EditState.Rectangle) {
				var position = e.GetPosition(editCanvas);
				if (position.X < 0) {
					Canvas.SetLeft(selectRect, 0);
					selectRect.Width = mouseDownPos.X;
				}
				else if (position.X > Image.Width) {
					Canvas.SetLeft(selectRect, mouseDownPos.X);
					selectRect.Width = Image.Width - mouseDownPos.X;
				}
				else {
					var dx = position.X - mouseDownPos.X;
					if (dx < 0)
						Canvas.SetLeft(selectRect, position.X);
					else
						Canvas.SetLeft(selectRect, mouseDownPos.X);
					selectRect.Width = Math.Abs(dx);
				}

				if (position.Y < 0) {
					Canvas.SetTop(selectRect, 0);
					selectRect.Height = mouseDownPos.Y;
				}
				else if (position.Y > Image.Height) {
					Canvas.SetTop(selectRect, mouseDownPos.Y);
					selectRect.Height = Image.Height - mouseDownPos.Y;
				}
				else {
					var dy = position.Y - mouseDownPos.Y;
					if (dy < 0)
						Canvas.SetTop(selectRect, position.Y);
					else
						Canvas.SetTop(selectRect, mouseDownPos.Y);
					selectRect.Height = Math.Abs(dy);
				}
			}
			else if (editState == EditState.Polygon) {

			}
			else if (editState == EditState.Eraser) {
				var centre = e.GetPosition(editCanvas);
				var size = new Vector(eraserRect.Width, eraserRect.Height);
				var position = centre - size / 2;
				Canvas.SetLeft(eraserRect, position.X);
				Canvas.SetTop(eraserRect, position.Y);
				var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
				Methods.EraseImage(Image, rect);
				// update the image by "N" frames per second
				if (stopwatch.ElapsedMilliseconds > 1000 / fps) {
					OnPropertyChanged(nameof(ImageSource));
					stopwatch.Restart();
				}
			}

			isMoving = false;
		}

		private void mainGrid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (editState == EditState.Eraser) {
				eraserRect.Visibility = Visibility.Hidden;

				var image = Image.Copy(); // save a copy to editManager
				if (EditManager.EditCommand.CanExecute((image, "erase image")))
					EditManager.EditCommand.Execute((image, "erase image"));
			}

			isEditting[editState] = false;
		}

		private void selectRect_KeyDown(object sender, KeyEventArgs e)
		{
			if (editState != EditState.Rectangle ||
				selectRect.Visibility != Visibility.Visible) {
				return;
			}
			if (e.Key == Key.Back || e.Key == Key.Delete) {
				var left = Canvas.GetLeft(selectRect);
				var top = Canvas.GetTop(selectRect);
				var rect = new Rectangle((int)left, (int)top, (int)selectRect.Width, (int)selectRect.Height);
				Methods.EraseImage(Image, rect);
				var image = Image.Copy();
				if (EditManager.EditCommand.CanExecute((image, "Delete rectangle region"))) {
					EditManager.EditCommand.Execute((image, "Delete rectangle region"));
				}
				selectRect.Visibility = Visibility.Hidden;
			}

			e.Handled = true;
		}
		private void PanZoomGrid_MouseWheel(object sender, double scale)
		{
			eraserRect.Width = eraserOriginalSize / scale;
			eraserRect.StrokeThickness = eraserOriginalBorderThickness / scale;
		}
	}

	public enum EditState
	{
		None,
		Eraser,
		Rectangle,
		Polygon,
	}
}
