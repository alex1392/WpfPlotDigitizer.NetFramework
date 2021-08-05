using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer2
{
	public static class Model
	{
		public static BitmapImage InputImage { get; set; }

		public readonly static List<Page> PageList = new List<Page>
		{
			new LoadPage(),
			new Page(),
		};
	}
}
