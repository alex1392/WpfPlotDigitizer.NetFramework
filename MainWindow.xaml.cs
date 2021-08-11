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

namespace WpfPlotDigitizer2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		private readonly Model model;

		public MainWindow(Model model, PageManager pageManager) : this()
		{
			this.model = model;
			this.PageManager = pageManager;
			PageNameList = pageManager.PageList.Select(p => p.GetType().Name);
			pageManager.GetPage<LoadPage>().PropertyChanged += LoadPage_PropertyChanged;
		}

		private void LoadPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var loadpage = sender as LoadPage;
			if (e.PropertyName == nameof(loadpage.Image)) {
				if (PageManager.NextCommand.CanExecute(null)) {
					PageManager.NextCommand.Execute(null);
				}
			}
		}

		public PageManager PageManager { get; private set; }
		public IEnumerable<string> PageNameList { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

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
