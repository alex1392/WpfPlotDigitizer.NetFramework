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
			var model = new AppData();
			var mainWindow = new MainWindow(model);
			mainWindow.Show();
#if DEBUG
			(mainWindow.PageManager.CurrentPage as LoadPage).SetImage(new BitmapImage(new Uri(@"C:\Users\alex\Desktop\Coding\WpfPlotDigitizer2\images\data.png")));
			var targetIndex = mainWindow.PageManager.PageList.FindIndex(p => p is EditPage);
			if (mainWindow.PageManager.GoToCommand.CanExecute(targetIndex))
			{
				mainWindow.PageManager.GoToCommand.Execute(targetIndex);
			}
#endif
		}
	}
}
