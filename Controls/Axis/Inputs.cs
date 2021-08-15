using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfPlotDigitizer.NetFramework
{
	[TypeConverter(typeof(InputsTypeConverter))]
	public class Inputs
	{
		public int? ClickCount { get; set; }
		public MouseButton? MouseButton { get; set; }
		public Collection<Key> Keys { get; set; } = new Collection<Key>();

		public bool IsEmpty => MouseButton == null && Keys.Count == 0;
		public bool IsValid(MouseButtonEventArgs e)
		{
			// check if there is required mouse button and if the button is pressed
			return (MouseButton != null && !((MouseButton)MouseButton).IsPressed()) ||
			  (ClickCount != null && e != null && e.ClickCount != ClickCount) ||
			  // check if there is any required keys and each key is pressed
			  (Keys.Count > 0 && !IsValid(Keys)) ? false : true;
		}

		private bool IsValid(Collection<Key> keys)
		{
			return keys.All(key => key.IsPressed());
		}
	}

	public static class InputsHelpers
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

		public static bool IsPressed(this Key key)
		{
			return Contain(Keyboard.GetKeyStates(key), (KeyStates.Down)) ? true : false;

			bool Contain(Enum enumA, Enum enumB)
			{
				var (a, b) = ConvertEnums(enumA, enumB);
				return (a & b) == b;
			}
			(ulong a, ulong b) ConvertEnums(Enum enumA, Enum enumB) => (Convert.ToUInt64(enumA), Convert.ToUInt64(enumB));
		}
		public static T[] GetAll<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToArray();
	}
}
