using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPlotDigitizer.NetFramework
{
	[ContentProperty(nameof(InnerContent))]
	/// <summary>
	/// Interaction logic for PanZoomViewer.xaml
	/// </summary>1
	public partial class PanZoomViewer : UserControl
	{
		public PanZoomViewer()
		{
			InitializeComponent();
			Loaded += PanZoomViewer_Loaded;
		}

		private void PanZoomViewer_Loaded(object sender, RoutedEventArgs e)
		{
			canvas.Width = content.ActualWidth;
			canvas.Height = content.ActualHeight;
		}

		public FrameworkElement InnerContent
		{
			get { return (FrameworkElement)GetValue(InnerContentProperty); }
			set { SetValue(InnerContentProperty, value); }
		}

		public static readonly DependencyProperty InnerContentProperty =
			DependencyProperty.Register("InnerContent", typeof(FrameworkElement), typeof(PanZoomViewer), new PropertyMetadata());

		public MouseButton PanMouseButton
		{
			get { return (MouseButton)GetValue(PanMouseButtonProperty); }
			set { SetValue(PanMouseButtonProperty, value); }
		}

		public static readonly DependencyProperty PanMouseButtonProperty =
			DependencyProperty.Register("PanMouseButton", typeof(MouseButton), typeof(PanZoomViewer), new PropertyMetadata());

		public ModifierKeys PanKeyModifier
		{
			get { return (ModifierKeys)GetValue(PanKeyModifierProperty); }
			set { SetValue(PanKeyModifierProperty, value); }
		}

		public static readonly DependencyProperty PanKeyModifierProperty =
			DependencyProperty.Register("PanKeyModifier", typeof(ModifierKeys), typeof(PanZoomViewer), new PropertyMetadata(ModifierKeys.None));


		public ModifierKeys ZoomKeyModifier
		{
			get { return (ModifierKeys)GetValue(ZoomKeyModifierProperty); }
			set { SetValue(ZoomKeyModifierProperty, value); }
		}

		public static readonly DependencyProperty ZoomKeyModifierProperty =
			DependencyProperty.Register("ZoomKeyModifier", typeof(ModifierKeys), typeof(PanZoomViewer), new PropertyMetadata(ModifierKeys.None));


		private static readonly Cursor panCursor = Cursors.Hand;
		private Point mouseAnchor;
		private double leftAnchor;
		private double topAnchor;
		private bool isPanning;

		private void ContentPresenter_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (PanInputCheck(content, e)) {
				mouseAnchor = e.GetPosition(canvas);
				leftAnchor = Canvas.GetLeft(content);
				topAnchor = Canvas.GetTop(content);
				Mouse.OverrideCursor = panCursor;
				content.CaptureMouse();
				isPanning = true;
			}
		}

		private void ContentPresenter_MouseMove(object sender, MouseEventArgs e)
		{
			if (isPanning && content.IsMouseCaptured) {
				var delta = e.GetPosition(canvas) - mouseAnchor;
				var scale = content.ActualWidth / canvas.ActualWidth;
				var toX = Math.Max(Math.Min(leftAnchor + delta.X, 0), canvas.ActualWidth * (1 - scale));
				var toY = Math.Max(Math.Min(topAnchor + delta.Y, 0), canvas.ActualHeight * (1 - scale));
				Canvas.SetLeft(content, toX);
				Canvas.SetTop(content, toY);
			}
		}

		private void ContentPresenter_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (content.IsMouseCaptured) {
				content.ReleaseMouseCapture();
				Mouse.OverrideCursor = null;
				isPanning = false;
			}
		}

		private void ContentPresenter_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.Modifiers.Contain(ZoomKeyModifier)) {
				// zoom speed
				var scale = content.ActualWidth / canvas.ActualWidth;

				var delta = scale * (e.Delta > 0 ? .2 : -.2);


				var relative = e.GetPosition(content);
				var absolute = e.GetPosition(canvas);
				//必須是scale先，translate後
				var ToScale = Math.Max(scale + delta, 1);
				var ToX = Math.Max(Math.Min(absolute.X - relative.X / scale * ToScale, 0), canvas.ActualWidth * (1 - ToScale));
				var ToY = Math.Max(Math.Min(absolute.Y - relative.Y / scale * ToScale, 0), canvas.ActualHeight * (1 - ToScale));

				content.Width = canvas.ActualWidth * ToScale;
				content.Height = canvas.ActualHeight * ToScale;
				Canvas.SetLeft(content, ToX);
				Canvas.SetTop(content, ToY);
			}
		}

		private bool PanInputCheck(FrameworkElement element, MouseButtonEventArgs e)
		{
			return (PanKeyModifier == ModifierKeys.None || IsKeyPressed(PanKeyModifier)) &&
				(IsMouseButtonPressed(PanMouseButton));

			bool IsKeyPressed(ModifierKeys key)
			{
				return key == ModifierKeys.None || Contains(Keyboard.Modifiers,key);

				bool Contains(ModifierKeys a, ModifierKeys b)
				{
					return (a & b) == b;
				}
			}

			bool IsMouseButtonPressed(MouseButton mouseButton)
			{
				switch (mouseButton) {
					default:
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
				}
			}
		}

	}

}
