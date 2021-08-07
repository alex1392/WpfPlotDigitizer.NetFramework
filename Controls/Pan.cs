using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

		private static readonly Cursor panCursor = new Uri(@"pack://application:,,,/Controls/pan.cur").ToCursor();
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
				var toX = Math.Clamp(translateAnchor.X + delta.X, 0, element.ActualWidth * (1 - scale.ScaleX));
				var toY = Math.Clamp(translateAnchor.Y + delta.Y, 0, element.ActualHeight * (1 - scale.ScaleY));
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

	public static class TransformExtensions
	{
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

		public static (TranslateTransform, ScaleTransform, RotateTransform, SkewTransform) SplitTransforms(this TransformCollection transforms) => (GetTranslate(transforms),
		  GetScale(transforms),
		  GetRotate(transforms),
		  GetSkew(transforms));

		public static TranslateTransform GetTranslate(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is TranslateTransform) as TranslateTransform;
		public static ScaleTransform GetScale(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is ScaleTransform) as ScaleTransform;
		public static RotateTransform GetRotate(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is RotateTransform) as RotateTransform;
		public static SkewTransform GetSkew(this TransformCollection transforms) => transforms.FirstOrDefault(tr => tr is SkewTransform) as SkewTransform;

		public static int IndexOfTranslate(this TransformCollection transforms) => transforms.ToList().FindIndex(tr => tr is TranslateTransform);
		public static int IndexOfScale(this TransformCollection transforms) => transforms.ToList().FindIndex(tr => tr is ScaleTransform);
		public static int IndexOfRotate(this TransformCollection transforms) => transforms.ToList().FindIndex(tr => tr is RotateTransform);
		public static int IndexOfSkew(this TransformCollection transforms) => transforms.ToList().FindIndex(tr => tr is SkewTransform);

		public static (int, int, int, int) GetIndices(this TransformCollection transforms) =>
		  (IndexOfTranslate(transforms),
		  IndexOfScale(transforms),
		  IndexOfRotate(transforms),
		  IndexOfSkew(transforms));
	}

	public static class AnimationExtensions
	{
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

		public static void BeginAnimation<PropertyType>(this IAnimatable animatable, DependencyProperty dp, PropertyType from, PropertyType to, double durationMs)
		{
			DependencyObject animation;
			var duration = TimeSpan.FromMilliseconds(durationMs);
			switch (to)
			{
				case int toInt:
					var fromInt = (int)Convert.ChangeType(from, typeof(int));
					animation = new Int32Animation(fromInt, toInt, duration);
					break;
				case double toDouble:
					var fromDouble = (double)Convert.ChangeType(from, typeof(double));
					animation = new DoubleAnimation(fromDouble, toDouble, duration);
					break;
				case Color toColor:
					var fromColor = (Color)Convert.ChangeType(from, typeof(Color));
					animation = new ColorAnimation(fromColor, toColor, duration);
					break;
				case Thickness toThickness:
					var fromThickness = (Thickness)Convert.ChangeType(from, typeof(Thickness));
					animation = new ThicknessAnimation(fromThickness, toThickness, duration);
					break;
				case Rect toRect:
					var fromRect = (Rect)Convert.ChangeType(from, typeof(Rect));
					animation = new RectAnimation(fromRect, toRect, duration);
					break;
				default:
					throw new NotSupportedException();
			}
			animatable.BeginAnimation(dp, animation as AnimationTimeline);
		}

	}
}
