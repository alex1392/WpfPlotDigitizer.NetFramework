using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace PlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly Model model;

		public event PropertyChangedEventHandler PropertyChanged;

		public PageManager PageManager { get; private set; }

		public IEnumerable<string> PageNameList { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		public MainWindow(Model model, PageManager pageManager) : this()
		{
			this.model = model;
			this.PageManager = pageManager;
			PageNameList = pageManager.PageList.Select(p => p.GetType().Name);
		}

		private void mainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			// disable frame navigation to prevent keyboard input conflict
			if (e.NavigationMode == NavigationMode.Back ||
				e.NavigationMode == NavigationMode.Forward) {
				e.Cancel = true;
			}
		}
	}
}