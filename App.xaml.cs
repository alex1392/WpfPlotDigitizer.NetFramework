﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer2
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
			var mainWindow = new MainWindow(model);
			mainWindow.Show();
#if DEBUG
			(mainWindow.CurrentPage as LoadPage).SetImage(new BitmapImage(new Uri(@"C:\Users\alex\Desktop\Coding\WpfPlotDigitizer2\images\data.png")));
#endif
		}
	}
}
