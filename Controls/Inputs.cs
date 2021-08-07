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

namespace WpfPlotDigitizer2
{
	[TypeConverter(typeof(InputsTypeConverter))]
	public class Inputs
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
	public class InputsTypeConverter : TypeConverter
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
			var input = new Inputs();
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
}
