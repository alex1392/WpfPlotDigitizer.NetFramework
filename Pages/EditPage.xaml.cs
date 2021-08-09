using Emgu.CV;
using Emgu.CV.Structure;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for EditPage.xaml
	/// </summary>
	public partial class EditPage : Page, INotifyPropertyChanged
	{
		private AppData data;
		private EditState editState;

		public event PropertyChangedEventHandler PropertyChanged;

		public EditPage()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;
		}

		public EditPage(AppData data) : this()
		{
			this.data = data;
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

		public EditState EditState
		{
			get => editState;
			set
			{
				editState = value;
				var canPanZoom = editState == EditState.None;
				Pan.SetIsEnabled(PanZoomGrid, canPanZoom);
				Zoom.SetIsEnabled(PanZoomGrid, canPanZoom);
			}
		}

		// need to update when undo/redo
		public Image<Rgba, byte> Image { get; private set; }

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index))
			{
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
				Image = EditManager.CurrentObject.Copy();
				OnPropertyChanged(nameof(ImageSource));
			}
		}
		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			Image = data.FilteredImage;
			EditManager = new EditManager<Image<Rgba, byte>>(Image.Copy());
			EditManager.PropertyChanged += EditManager_PropertyChanged;
		}

		private void EditPage_Unloaded(object sender, RoutedEventArgs e)
		{
			data.EdittedImage = Image;
		}

		private void EraserButton_Checked(object sender, RoutedEventArgs e)
		{
			RectButton.IsChecked = false;
			PolyButton.IsChecked = false;
			EditState = EditState.Eraser;
		}

		private void RectButton_Checked(object sender, RoutedEventArgs e)
		{
			EraserButton.IsChecked = false;
			PolyButton.IsChecked = false;
			EditState = EditState.Rectangle;
		}

		private void PolyButton_Checked(object sender, RoutedEventArgs e)
		{
			RectButton.IsChecked = false;
			EraserButton.IsChecked = false;
			EditState = EditState.Polygon;
		}

		private void StateButton_Unchecked(object sender, RoutedEventArgs e)
		{
			EditState = EditState.None;
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

		private Point mouseDownPos;
		private bool isMoving;
		private readonly Dictionary<EditState, bool> isEditting = new Dictionary<EditState, bool>
		{
			{ EditState.Eraser, false },
			{ EditState.Rectangle, false },
			{ EditState.Polygon, false },
		};
		private void selectCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (editState == EditState.Rectangle)
			{
				mouseDownPos = e.GetPosition(selectCanvas);
				Canvas.SetLeft(selectRect, mouseDownPos.X);
				Canvas.SetTop(selectRect, mouseDownPos.Y);
				selectRect.Width = 0;
				selectRect.Height = 0;
				selectRect.Visibility = Visibility.Visible;

				isEditting[EditState.Rectangle] = true;
			}
			else if (editState == EditState.Polygon)
			{

				isEditting[EditState.Polygon] = true;
			}
		}

		private void selectCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (!isEditting.ContainsKey(editState) ||
				!isEditting[editState] ||
				isMoving)
				return;
			isMoving = true;

			var position = e.GetPosition(selectCanvas);
			if (editState == EditState.Rectangle)
			{
				var dx = position.X - mouseDownPos.X;
				if (dx < 0)
					Canvas.SetLeft(selectRect, position.X);
				else
					Canvas.SetLeft(selectRect, mouseDownPos.X);
				selectRect.Width = Math.Abs(dx);

				var dy = position.Y - mouseDownPos.Y;
				if (dy < 0)
					Canvas.SetTop(selectRect, position.Y);
				else
					Canvas.SetTop(selectRect, mouseDownPos.Y);
				selectRect.Height = Math.Abs(dy);
			}
			else if (editState == EditState.Polygon)
			{

			}

			isMoving = false;
		}

		private void selectCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			isEditting[editState] = false;
		}


		private void eraserCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (editState != EditState.Eraser)
				return;
			eraserRect.Visibility = Visibility.Visible;
			isEditting[EditState.Eraser] = true;
			eraserCanvas_MouseMove(sender, e);
		}

		private void eraserCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (editState != EditState.Eraser ||
				!isEditting[EditState.Eraser] ||
				isMoving)
				return;
			isMoving = true;

			var centre = e.GetPosition(eraserCanvas);
			var size = new Vector(eraserRect.Width, eraserRect.Height);
			var position = centre - size / 2;
			Canvas.SetLeft(eraserRect, position.X);
			Canvas.SetTop(eraserRect, position.Y);
			var rect = new System.Drawing.Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
			// erase image
			CvInvoke.Rectangle(Image, rect, new Rgba().MCvScalar, -1);
			OnPropertyChanged(nameof(ImageSource));

			isMoving = false;
		}

		private void eraserCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (editState != EditState.Eraser)
				return;
			isEditting[editState] = false;
			eraserRect.Visibility = Visibility.Hidden;

			// save a copy to editManager
			var image = Image.Copy();
			if (EditManager.EditCommand.CanExecute((image, "erase image")))
				EditManager.EditCommand.Execute((image, "erase image"));
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
