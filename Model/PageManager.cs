using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfPlotDigitizer2
{
	public class PageManager : INotifyPropertyChanged
	{
		private PageManager()
		{
			BackCommand = new RelayCommand(GoBack, CanGoBack);
			NextCommand = new RelayCommand(GoNext, CanGoNext);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
		}

		public PageManager(List<Page> pageList) : this()
		{
			PageList = pageList;
		}

		public int PageIndex { get; set; } = 0;
		public List<Page> PageList { get; private set; }
		public Page CurrentPage => PageList[PageIndex];

		public RelayCommand BackCommand { get; private set; }
		public RelayCommand NextCommand { get; private set; }
		public RelayCommand<int> GoToCommand { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

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
