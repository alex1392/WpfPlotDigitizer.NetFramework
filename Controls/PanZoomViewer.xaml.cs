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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for PanZoomViewer.xaml
	/// </summary>
	public partial class PanZoomViewer : UserControl
	{
		public PanZoomViewer()
		{
			InitializeComponent();
		}

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
		private bool isPanning;

		private void ContentPresenter_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (PanInputCheck(content, e)) {
				mouseAnchor = e.GetPosition(mainCanvas);
				Mouse.OverrideCursor = panCursor;
				content.CaptureMouse();
				isPanning = true;
			}
		}

		private void ContentPresenter_MouseMove(object sender, MouseEventArgs e)
		{
			if (isPanning && content.IsMouseCaptured) {
				var delta = e.GetPosition(mainCanvas) - mouseAnchor;
				var scale = content.ActualWidth / mainCanvas.ActualWidth;
				var toX = PanZoomHelpers.Clamp(Canvas.GetLeft(content) + delta.X, 0, content.ActualWidth * (1 - scale));
				var toY = PanZoomHelpers.Clamp(Canvas.GetTop(content) + delta.Y, 0, content.ActualHeight * (1 - scale));
			}
		}

		private void ContentPresenter_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void ContentPresenter_MouseWheel(object sender, MouseWheelEventArgs e)
		{

		}
		private void ContentPresenter_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private bool PanInputCheck(FrameworkElement element, MouseButtonEventArgs e)
		{
			return (PanKeyModifier == ModifierKeys.None || IsKeyPressed(PanKeyModifier)) &&
				(IsMouseButtonPressed(PanMouseButton));

			bool IsKeyPressed(ModifierKeys key)
			{
				return key == ModifierKeys.None || Keyboard.Modifiers.Contain(key);
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

	public static class PanZoomViewerHelper
	{

	}
}
