using GalaSoft.MvvmLight.Command;
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
			BackCommand = new RelayCommand(GoBack, CanGoBack);
			NextCommand = new RelayCommand(GoNext, CanGoNext);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
		}

		private AppData data;
		public MainWindow(AppData data) : this()
		{
			this.data = data;
			PageList = new List<Page>
			{
				new LoadPage(data),
				new AxisLimitPage(data),
				new AxisPage(data),
				new FilterPage(data),
				new EditPage(data),
				new PreviewPage(data),
			};
		}

		public int PageIndex { get; private set; } = 0;
		public readonly List<Page> PageList;
		public Page CurrentPage => PageList[PageIndex];

		public event PropertyChangedEventHandler PropertyChanged;

		public RelayCommand BackCommand { get; set; }
		public RelayCommand NextCommand { get; set; }
		public RelayCommand<int> GoToCommand { get; set; }
		private void GoBack()
		{
			PageIndex--;
			BackCommand.RaiseCanExecuteChanged();
			NextCommand.RaiseCanExecuteChanged();
		}
		private bool CanGoBack()
		{
			return PageIndex > 0;
		}
		private void GoNext()
		{
			PageIndex++;
			BackCommand.RaiseCanExecuteChanged();
			NextCommand.RaiseCanExecuteChanged();
		}
		private bool CanGoNext()
		{
			return PageIndex < PageList.Count - 1;
		}
		private void GoTo(int targetIndex)
		{
			while (PageIndex < targetIndex)
			{
				// wait for the UI thread to catch up
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => GoNext())).Wait();
			}
			while (PageIndex > targetIndex)
			{
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => GoBack())).Wait();
			}
		}
		private bool CanGoTo(int targetIndex)
		{
			return targetIndex >= 0 && targetIndex < PageList.Count;
		}
	}
}
