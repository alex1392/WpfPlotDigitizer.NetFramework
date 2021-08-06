using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using static WpfPlotDigitizer2.Math;

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Axis.xaml 的互動邏輯
	/// </summary>
	public partial class Axis : UserControl, INotifyPropertyChanged
	{
		public Axis()
		{
			InitializeComponent();
			gridMain.DataContext = this;
		}

		#region DPs
		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			nameof(Stroke),
			typeof(Brush),
			typeof(Axis),
			new PropertyMetadata(new SolidColorBrush(Colors.Red)));

		public double AxisLeft
		{
			get => (double)GetValue(AxisLeftProperty);
			set => SetValue(AxisLeftProperty, Clamp(value, AxisRight - tol, 0));
		}
		public static readonly DependencyProperty AxisLeftProperty = DependencyProperty.Register(
			nameof(AxisLeft),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisLeftChanged));

		public double AxisTop
		{
			get => (double)GetValue(AxisTopProperty);
			set => SetValue(AxisTopProperty, Clamp(value, AxisBottom - tol, 0));
		}
		public static readonly DependencyProperty AxisTopProperty = DependencyProperty.Register(
			nameof(AxisTop),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisTopChanged));

		public double AxisWidth
		{
			get => (double)GetValue(AxisWidthProperty);
			set => SetValue(AxisWidthProperty, Clamp(value, double.MaxValue, tol));
		}
		public static readonly DependencyProperty AxisWidthProperty = DependencyProperty.Register(
			nameof(AxisWidth),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisWidthChanged));

		public double AxisHeight
		{
			get => (double)GetValue(AxisHeightProperty);
			set => SetValue(AxisHeightProperty, Clamp(value, double.MaxValue, tol));
		}
		public static readonly DependencyProperty AxisHeightProperty = DependencyProperty.Register(
			nameof(AxisHeight),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisHeightChanged));
		public ImageSource ImageSource
		{
			get => (ImageSource)GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			nameof(ImageSource),
			typeof(ImageSource),
			typeof(Axis),
			new PropertyMetadata(default(ImageSource), OnImageSourceChanged));

		public CycInput Input
		{
			get => (CycInput)GetValue(InputProperty);
			set => SetValue(InputProperty, value);
		}
		public static readonly DependencyProperty InputProperty = DependencyProperty.Register(
			nameof(Input),
			typeof(CycInput),
			typeof(Axis),
			new PropertyMetadata(new CycInput()));

		private static void OnAxisLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}
		private static void OnAxisTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}
		private static void OnAxisWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}
		private static void OnAxisHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}
		private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.image = axis.ImageSource as BitmapSource;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}
		#endregion

		public Thickness AxisMargin => new Thickness(AxisLeft, AxisTop, 0, 0);
		public Rect AxisRelative => 
			image == null ? 
			new Rect() : 
			new Rect(AxisLeft / image.PixelWidth, 
				AxisTop / image.PixelHeight, 
				AxisWidth / image.PixelWidth, 
				AxisHeight / image.PixelHeight);
		public double AxisRight => AxisLeft + AxisWidth;
		public double AxisBottom => AxisTop + AxisHeight;

		private BitmapSource image;
		private double tol = 10;
		private bool IsAdjust = false;
		private AdjustType State = AdjustType.None;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private AdjustType GetState(Point mousePos)
		{
			var state = new AdjustType();
			if (ApproxEqual(mousePos.X, AxisLeft, tol) &&
			  IsIn(mousePos.Y, AxisBottom + tol, AxisTop - tol))
				state = (AdjustType)state.Add(AdjustType.Left);
			if (ApproxEqual(mousePos.Y, AxisTop, tol) &&
			  IsIn(mousePos.X, AxisRight + tol, AxisLeft - tol))
				state = (AdjustType)state.Add(AdjustType.Top);
			if (ApproxEqual(mousePos.X, AxisRight, tol) &&
			  IsIn(mousePos.Y, AxisBottom + tol, AxisTop - tol))
				state = (AdjustType)state.Add(AdjustType.Right);
			if (ApproxEqual(mousePos.Y, AxisBottom, tol) &&
			  IsIn(mousePos.X, AxisRight + tol, AxisLeft - tol))
				state = (AdjustType)state.Add(AdjustType.Bottom);
			return state;
		}
		private void UpdateCursor(AdjustType state)
		{
			switch (state)
			{
				default:
				case AdjustType.None:
					Cursor = Cursors.Arrow;
					break;
				case AdjustType.Left:
				case AdjustType.Right:
					Cursor = Cursors.SizeWE;
					break;
				case AdjustType.Top:
				case AdjustType.Bottom:
					Cursor = Cursors.SizeNS;
					break;
				case AdjustType.LeftTop:
				case AdjustType.RightBottom:
					Cursor = Cursors.SizeNWSE;
					break;
				case AdjustType.RightTop:
				case AdjustType.LeftBottom:
					Cursor = Cursors.SizeNESW;
					break;
			}
		}
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!InputCheck(e))
				return;
			var mousePos = e.GetPosition(gridMain);
			State = GetState(mousePos);
			UpdateCursor(State);

			// Initialize Adjust
			if (State != (AdjustType.None))
			{
				IsAdjust = true;
				CaptureMouse();

				// block other events
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			IsAdjust = false;
			ReleaseMouseCapture();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			var mousePos = e.GetPosition(gridMain);
			// is not adjusting, just update the cursor
			if (!IsAdjust)
			{
				UpdateCursor(GetState(mousePos));
				return;
			}
			// avoid mouse go outside the grid
			if (!(IsIn(mousePos.X, gridMain.ActualWidth, 0) &&
				  IsIn(mousePos.Y, gridMain.ActualHeight, 0)))
				return;
			// adjust 
			if (State.Contain(AdjustType.Left))
			{
				var delta = mousePos.X - AxisLeft;
				AxisLeft = mousePos.X; // must be checked earlier than width
				AxisWidth -= delta;
			}
			if (State.Contain(AdjustType.Top))
			{
				var delta = mousePos.Y - AxisTop;
				AxisTop = mousePos.Y;
				AxisHeight -= delta;
			}
			if (State.Contain(AdjustType.Right))
				AxisWidth = mousePos.X - AxisLeft;
			if (State.Contain(AdjustType.Bottom))
				AxisHeight = mousePos.Y - AxisTop;
		}

		private bool InputCheck(EventArgs e)
		{
			var arg = e is MouseButtonEventArgs mbe ? mbe : null;
			return !Input.IsEmpty && Input.IsValid(arg) ? true : false;
		}
	}

	public enum AdjustType
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		LeftTop = Left | Top,
		RightTop = Right | Top,
		LeftBottom = Left | Bottom,
		RightBottom = Right | Bottom,
	}

	[TypeConverter(typeof(CycInputTypeConverter))]
	public class CycInput
	{
		public int? ClickCount { get; set; }
		public MouseButton? MouseButton { get; set; }
		public Collection<Key> InputKeys { get; set; } = new Collection<Key>();

		public bool IsEmpty => MouseButton == null && InputKeys.Count == 0 ? 
			true : false;
		public bool IsValid(MouseButtonEventArgs e)
		{
			// check if there is required mouse button and if the button is pressed
			return (MouseButton != null && !((MouseButton)MouseButton).IsPressed()) ||
			  (ClickCount != null && e != null && e.ClickCount != ClickCount) ||
			// check if there is any required keys and each key is pressed
			  (InputKeys.Count > 0 && !IsValid(InputKeys)) ? false : true;
		}

		private bool IsValid(Collection<Key> keys)
		{
			return keys.All(key => key.IsPressed());
		}
	}

	/// <summary>
	/// "mouse" and "key" as indicator
	/// ':' and ';' as starter and finisher
	/// ',' as seperator
	/// numbers in "mouse" section is ClickCount
	/// </summary>
	/// <example>
	/// mouse: left, 2; key: leftCtrl, c
	/// </example>
	public class CycInputTypeConverter : TypeConverter
	{
		public static readonly string mouseStr = "mouse";
		public static readonly string keyStr = "key";

		private string GetSubStr(string str, int endIndex)
		{
			var start = str.IndexOf(':', endIndex);
			var end = str.IndexOf(';', endIndex);
			if (start < 0) // if no starter
				start = endIndex + 1;
			if (end < 0) // if no finisher
				end = str.Length;
			start++; //get rid of ':'
			return str.Substring(start, end - start);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var input = new CycInput();
			var str = value.ToString().ToLower();
			var mouseStart = str.IndexOf(mouseStr);
			var keyStart = str.IndexOf(keyStr);
			if (mouseStart >= 0)
			{
				var mouseEnd = mouseStart + mouseStr.Length;
				var mouseSubStr = GetSubStr(str, mouseEnd);
				var mouseSubStrs = mouseSubStr.Split(',');
				var mouseAll = EnumHelpers.GetAll<MouseButton>();
				foreach (var subStr in mouseSubStrs)
				{
					var trimStr = subStr.Trim();
					if (trimStr.Length == 1 && char.IsDigit(trimStr[0]))
						input.ClickCount = int.Parse(trimStr);
					try
					{
						input.MouseButton = mouseAll.First(mb => trimStr == mb.ToString().ToLower());
					}
					catch (InvalidOperationException) { }//no matched mouse button
				}
			}

			if (keyStart >= 0) // if key specified
			{
				var keyEnd = keyStart + keyStr.Length;
				var keySubStr = GetSubStr(str, keyEnd);
				var keySubStrs = keySubStr.Split(',');
				var keyAll = EnumHelpers.GetAll<Key>();
				foreach (var subStr in keySubStrs)
				{
					var trimStr = subStr.Trim();
					var keys = keyAll.Where(key => trimStr == key.ToString().ToLower());
					foreach (var key in keys)
					{
						input.InputKeys.Add(key);
					}
				}
			}

			return input;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}
	}

	public static class InputExtensions
	{
		public static bool IsPressed(this MouseButton mouseButton)
		{
			switch (mouseButton)
			{
				case MouseButton.Left:
					return Mouse.LeftButton == MouseButtonState.Pressed;
				case MouseButton.Middle:
					return Mouse.MiddleButton == MouseButtonState.Pressed;
				case MouseButton.Right:
					return Mouse.RightButton == MouseButtonState.Pressed;
				case MouseButton.XButton1:
					return Mouse.XButton1 == MouseButtonState.Pressed;
				case MouseButton.XButton2:
					return Mouse.XButton2 == MouseButtonState.Pressed;
				default:
					return false;
			}
		}

		public static bool IsEmpty(this Key key) => key == Key.None ? true : false;
		public static bool IsPressed(this Key key) => Keyboard.GetKeyStates(key).Contain(KeyStates.Down) ? true : false;
	}

	public static class EnumExtensions
	{
		public static dynamic Add(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a | b;
		}

		public static dynamic Remove(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a & ~b;
		}

		public static bool Contain(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return (a & b) == b;
		}

		public static bool GreaterThan(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a > b;
		}

		public static bool LessThan(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a < b;
		}

		public static int Count(this Enum @enum) => Enum.GetValues(@enum.GetType()).Length;

		#region Helper
		private static void CheckType(Enum enumA, Enum enumB)
		{
			if (enumA.GetType() != enumB.GetType())
				throw new ArgumentException("Type Mismatch");
		}
		private static (ulong a, ulong b) ConvertEnums(Enum enumA, Enum enumB) => (Convert.ToUInt64(enumA), Convert.ToUInt64(enumB));
		#endregion
	}

	public static class EnumHelpers
	{
		public static int Count<T>() => Enum.GetValues(typeof(T)).Length;

		public static T[] GetAll<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToArray();
	}

	public static class Math
	{
		public static double Clamp(double value, double Max, double Min)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);

			if (value > Max)
				return Max;
			else if (value < Min)
				return Min;
			else
				return value;
		}

		public static void Swap<T>(ref T x, ref T y) => (x, y) = (y, x);

		/// <summary>
		/// 判斷<paramref name="value"/>是否位於閉區間[<paramref name="Max"/>,<paramref name="Min"/>]中。<paramref name="excludeBoundary"/>為真時，改為判斷開區間(<paramref name="Max"/>,<paramref name="Min"/>)。
		/// </summary>
		public static bool IsIn(double value, double Max, double Min, bool excludeBoundary = false)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);
			if (!excludeBoundary)
				return (value <= Max && value >= Min) ? true : false;
			else
				return (value < Max && value > Min) ? true : false;
		}

		public static bool IsIn(double value, double Max, double Min, bool excludeMax, bool excludeMin)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);
			var inMax = excludeMax ? value < Max : value <= Max;
			var inMin = excludeMin ? value > Min : value >= Min;
			return inMax && inMin;
		}

		/// <summary>
		/// 判斷<paramref name="A"/>是否約等於<paramref name="B"/>。
		/// </summary>
		/// <param name="tol">容許誤差。</param>
		public static bool ApproxEqual(double A, double B, double tol)
		{
			return IsIn(A, B + tol, B - tol);
		}
	}
}
