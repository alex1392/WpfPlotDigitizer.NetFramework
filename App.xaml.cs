using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var model = new Model();
			var pageList = new List<Page>
			{
				new TestPage(),
				new LoadPage(model),
				new AxisLimitPage(model),
				new AxisPage(model),
				new FilterPage(model),
				new EditPage(model),
				new PreviewPage(model),
			};
			var pageManager = new PageManager(pageList);
			var mainWindow = new MainWindow(model, pageManager);
			mainWindow.Show();
#if DEBUG
			if (false) {
				model.InputBitmapImage = new BitmapImage(new Uri(@"C:\Users\alex\Desktop\Coding\WpfPlotDigitizer\images\Screenshot 2021-06-26 231058.png"));
				model.AxisLimit = new Rect(900, 0, 70, 20);
				model.AxisLogBase = new Point(0, 0);
				model.AxisLocation = new Rect(138, 100, 632, 399);
				model.Filter = (Color.FromRgb(0, 0, 0), Color.FromRgb(126, 254, 254));
				model.DataType = DataType.Discrete;

				if (pageManager.GoToByTypeCommand.CanExecute(typeof(PreviewPage))) {
					pageManager.GoToByTypeCommand.Execute(typeof(PreviewPage));
				} 
			}
#endif
		}
	}
}
