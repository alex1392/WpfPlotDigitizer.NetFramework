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

		private readonly Model data;

		public MainWindow(Model data) : this()
		{
			this.data = data;
			var pageList = new List<Page>
			{
				new LoadPage(data),
				new AxisLimitPage(data),
				new AxisPage(data),
				new FilterPage(data),
				new EditPage(data),
				new PreviewPage(data),
			};
			PageNameList = pageList.Select(p => p.GetType().Name);
			PageManager = new PageManager(pageList);
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
