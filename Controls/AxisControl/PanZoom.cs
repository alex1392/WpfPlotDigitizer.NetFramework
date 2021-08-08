using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// 提供平移相依屬性的靜態類別。
	/// </summary>
	public static class Pan
	{
		#region Dependency Properties
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
		  "IsEnabled",
		  typeof(bool),
		  typeof(Pan),
		  new PropertyMetadata(default(bool), OnIsEnabledChanged));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(UIElement element)
		  => (bool)element.GetValue(IsEnabledProperty);
		public static void SetIsEnabled(UIElement element, bool value)
		  => element.SetValue(IsEnabledProperty, value);

		public static readonly DependencyProperty InputProperty = DependencyProperty.RegisterAttached(
			"Input",
			typeof(Inputs),
			typeof(Pan),
			new PropertyMetadata(new Inputs()));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		[TypeConverter(typeof(InputsTypeConverter))]
		public static Inputs GetInput(DependencyObject obj)
			=> (Inputs)obj.GetValue(InputProperty);
		public static void SetInput(DependencyObject obj, Inputs value)
			=> obj.SetValue(InputProperty, value);

		public static readonly DependencyProperty ClipToParentProperty = DependencyProperty.RegisterAttached(
			"ClipToParent",
			typeof(bool),
			typeof(Pan),
			new PropertyMetadata(default(bool)));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetClipToParent(DependencyObject obj)
			=> (bool)obj.GetValue(ClipToParentProperty);
		public static void SetClipToParent(DependencyObject obj, bool value)
			=> obj.SetValue(ClipToParentProperty, value);
		#endregion

		private static readonly Cursor panCursor = new Uri(@"/Controls/AxisControl/pan.cur", UriKind.Relative).ToCursor();
		private static Cursor ToCursor(this Uri uri) => new Cursor(Application.GetResourceStream(uri).Stream);
		private static Cursor cursorCache;
		private static Point mouseAnchor;
		private static TranslateTransform translate;
		private static Point translateAnchor;
		private static bool IsPanning;

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is FrameworkElement element))
				throw new NotSupportedException($"Can only set the {IsEnabledProperty} attached behavior on a UIElement.");

			if ((bool)e.NewValue)
			{
				element.MouseDown += Element_MouseDown;
				element.MouseUp += Element_MouseUp;
				element.MouseMove += Element_MouseMove;
				element.EnsureTransforms();
				if (element.Parent != null && GetClipToParent(element))
					element.Parent.SetValue(UIElement.ClipToBoundsProperty, true);
			}
			else
			{
				element.MouseDown -= Element_MouseDown;
				element.MouseUp -= Element_MouseUp;
				element.MouseMove -= Element_MouseMove;
			}
		}

		private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (InputCheck(element, e))
			{
				var transforms = (element.RenderTransform as TransformGroup).Children;
				translate = transforms.GetTranslate();
				mouseAnchor = e.GetAbsolutePosition(element);
				translateAnchor = new Point(translate.X, translate.Y);
				cursorCache = element.Cursor;
				element.Cursor = panCursor;
				element.CaptureMouse();
				IsPanning = true;
			}
		}
		private static void Element_MouseUp(object sender, MouseButtonEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element.IsMouseCaptured)
			{
				element.ReleaseMouseCapture();
				element.Cursor = cursorCache;
				IsPanning = false;
			}
		}
		private static void Element_MouseMove(object sender, MouseEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element.IsMouseCaptured && IsPanning)
			{
				var delta = e.GetAbsolutePosition(element) - mouseAnchor;
				var scale = (element.RenderTransform as TransformGroup).Children.GetScale();
				var toX = PanZoomHelpers.Clamp(translateAnchor.X + delta.X, 0, element.ActualWidth * (1 - scale.ScaleX));
				var toY = PanZoomHelpers.Clamp(translateAnchor.Y + delta.Y, 0, element.ActualHeight * (1 - scale.ScaleY));
				translate.BeginAnimation(TranslateTransform.XProperty, toX, 0);
				translate.BeginAnimation(TranslateTransform.YProperty, toY, 0);
			}

		}

		private static bool InputCheck(FrameworkElement element, EventArgs e)
		{
			var input = GetInput(element);
			var arg = e is MouseButtonEventArgs mbe ? mbe : null;
			return (!input.IsEmpty && input.IsValid(arg)) ? true : false;
		}

	}

	/// <summary>
	/// 提供縮放相依屬性的靜態類別。
	/// </summary>
	public static class Zoom
	{
		#region Dependency Properties
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
		  "IsEnabled",
		  typeof(bool),
		  typeof(Zoom),
		  new PropertyMetadata(OnIsEnabledChanged));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(UIElement element)
		  => (bool)element.GetValue(IsEnabledProperty);
		public static void SetIsEnabled(UIElement element, bool value)
		  => element.SetValue(IsEnabledProperty, value);

		public static readonly DependencyProperty IsLeaveResetProperty = DependencyProperty.RegisterAttached(
		  "IsLeaveReset",
		  typeof(bool),
		  typeof(Zoom),
		  new PropertyMetadata(default(bool)));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsLeaveReset(UIElement element)
		  => (bool)element.GetValue(IsLeaveResetProperty);
		public static void SetIsLeaveReset(UIElement element, bool value)
		  => element.SetValue(IsLeaveResetProperty, value);

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached(
		  "Maximum",
		  typeof(double),
		  typeof(Zoom),
		  new PropertyMetadata(5d, OnMaximumChanged));
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static double GetMaximum(UIElement element)
		  => (double)element.GetValue(MaximumProperty);
		public static void SetMaximum(UIElement element, double value)
		  => element.SetValue(MaximumProperty, value);
		#endregion

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is FrameworkElement element))
				throw new NotSupportedException($"Can only set the {IsEnabledProperty} attached behavior on a UIElement.");

			if ((bool)e.NewValue)
			{
				element.MouseWheel += Element_MouseWheel;
				if (GetIsLeaveReset(element))
					element.MouseLeave += Element_MouseLeave;
				element.EnsureTransforms();
				element.RenderTransformOrigin = new Point(0, 0);
				element.Parent.SetValue(UIElement.ClipToBoundsProperty, true);
				if (element.Parent is Panel parent)
					parent.Background = CrossboardBrush;
				else if (element.Parent is Border border)
					border.Background = CrossboardBrush;
			}
			else
			{
				element.MouseWheel -= Element_MouseWheel;
				element.MouseLeave -= Element_MouseLeave;
			}
		}

		private static DrawingBrush CrossboardBrush { get; } = new DrawingBrush
		{
			TileMode = TileMode.Tile,
			Viewport = new Rect(0, 0, 32, 32),
			ViewportUnits = BrushMappingMode.Absolute,
			Drawing = new GeometryDrawing
			{
				Brush = Brushes.LightGray,
				Geometry = Geometry.Parse("M0,0 H16 V16 H32 V32 H16 V16 H0Z"),
			}
		};

		private static double LeaveTime = 1;
		private static double WheelTime = 0.1;

		private static void Element_MouseLeave(object sender, MouseEventArgs e)
		{
			var element = sender as UIElement;
			var transforms = (element.RenderTransform as TransformGroup).Children;
			var translate = transforms.GetTranslate();
			var scale = transforms.GetScale();
			translate.BeginAnimation(TranslateTransform.XProperty, 1d, LeaveTime);
			translate.BeginAnimation(TranslateTransform.YProperty, 1d, LeaveTime);
			scale.BeginAnimation(ScaleTransform.ScaleXProperty, 1d, LeaveTime);
			scale.BeginAnimation(ScaleTransform.ScaleYProperty, 1d, LeaveTime);
		}
		private static void Element_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var element = sender as FrameworkElement;
			var parent = element.Parent as FrameworkElement;
			var transforms = (element.RenderTransform as TransformGroup).Children;
			var translate = transforms.GetTranslate();
			var scale = transforms.GetScale();

			//ZoomSpeed
			var zoom = e.Delta > 0 ? .2 : -.2;

			var relative = e.GetPosition(element);
			var absolute = e.GetAbsolutePosition(element);
			//必須是scale先，translate後
			var ToScale = PanZoomHelpers.Clamp(scale.ScaleX + zoom, GetMaximum(element), 1);
			var ToX = PanZoomHelpers.Clamp(absolute.X - relative.X * ToScale, 0, element.ActualWidth * (1 - ToScale));
			var ToY = PanZoomHelpers.Clamp(absolute.Y - relative.Y * ToScale, 0, element.ActualHeight * (1 - ToScale));
			scale.BeginAnimation(ScaleTransform.ScaleXProperty, ToScale, WheelTime);
			scale.BeginAnimation(ScaleTransform.ScaleYProperty, ToScale, WheelTime);
			translate.BeginAnimation(TranslateTransform.XProperty, ToX, WheelTime);
			translate.BeginAnimation(TranslateTransform.YProperty, ToY, WheelTime);

		}
		private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var value = (double)e.NewValue;
			if (value < 1)
			{
				throw new InvalidOperationException($"{MaximumProperty} must be greater than inital scale (1).");
			}
		}
	}

	public static class PanZoomHelpers
	{
		/// <summary>
		/// 取得滑鼠相對於<paramref name="element"/>在螢幕上的座標，此結果不會受到<see cref="UIElement.RenderTransform"/>的影響。
		/// </summary>
		public static Point GetAbsolutePosition(this MouseEventArgs e, UIElement element)
		{
			var transformsTemplate = (element.RenderTransform as TransformGroup).Children;
			var transformsIdentity = new TransformCollection();
			// 重設UIElement的transforms
			(element.RenderTransform as TransformGroup).Children = transformsIdentity;
			// 取得座標
			var absolute = e.GetPosition(element);
			// 復原transforms
			(element.RenderTransform as TransformGroup).Children = transformsTemplate;
			return absolute;
		}
		public static void EnsureTransforms(this UIElement element)
		{
			var transform = element.RenderTransform;
			if (transform is TransformGroup group &&
			  group.Children.Count == 4 &&
			  group.Children[0] is ScaleTransform &&
			  group.Children[1] is TranslateTransform &&
			  group.Children[2] is RotateTransform &&
			  group.Children[3] is SkewTransform)
				return; //需要確認模式，以保證使用此方法檢驗過的element都具有相同的transform，方便使用者紀錄transformCache
			element.RenderTransform = new TransformGroup
			{
				Children = new TransformCollection
		{
          // 必須是scale先，translate後
          new ScaleTransform(),
		  new TranslateTransform(),
		  new RotateTransform(),
		  new SkewTransform(),
		},
			};
			element.RenderTransformOrigin = new Point(0, 0);
		}
		public static TranslateTransform GetTranslate(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is TranslateTransform) as TranslateTransform;
		public static ScaleTransform GetScale(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is ScaleTransform) as ScaleTransform;
		/// <summary>
		/// 針對<paramref name="animatable"/>執行泛型動畫。
		/// </summary>
		/// <typeparam name="PropertyType">執行動畫的屬性型別。</typeparam>
		/// <param name="animatable">要執行動畫的個體。</param>
		/// <param name="dp">要執行動畫的屬性。</param>
		/// <param name="to">屬性改變的目標值。</param>
		/// <param name="durationMs">動畫的時長。</param>
		public static void BeginAnimation<PropertyType>(this IAnimatable animatable, DependencyProperty dp, PropertyType to, double durationMs)
		{
			DependencyObject animation;
			var duration = TimeSpan.FromMilliseconds(durationMs);
			switch (to)
			{
				case int i:
					animation = new Int32Animation(i, duration);
					break;
				case double d:
					animation = new DoubleAnimation(d, duration);
					break;
				case Color color:
					animation = new ColorAnimation(color, duration);
					break;
				case Thickness thickness:
					animation = new ThicknessAnimation(thickness, duration);
					break;
				case Rect rect:
					animation = new RectAnimation(rect, duration);
					break;
				default:
					throw new NotSupportedException();
			}
			animatable.BeginAnimation(dp, animation as AnimationTimeline);
		}
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
		 
			void Swap<T>(ref T x, ref T y) => (x, y) = (y, x);
		}

	}
}
