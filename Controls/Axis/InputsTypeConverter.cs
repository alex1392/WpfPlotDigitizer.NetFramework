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
				var mouseAll = InputsHelpers.GetAll<MouseButton>();
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
				var keyAll = InputsHelpers.GetAll<Key>();
				foreach (var subStr in keySubStrs)
				{
					var trimStr = subStr.Trim();
					var keys = keyAll.Where(key => trimStr == key.ToString().ToLower());
					foreach (var key in keys)
					{
						input.Keys.Add(key);
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
}
