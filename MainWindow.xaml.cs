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
		}

		private Model model;
		public MainWindow(Model model) : this()
		{
			this.model = model;
			PageList = new List<Page>
			{
				new LoadPage(model),
				new AxisPage(model),
				new AxisLimitPage(model),
			};
		}

		private int PageIndex { get; set; } = 0;
		private readonly List<Page> PageList;
		public Page CurrentPage => PageList[PageIndex];

		public event PropertyChangedEventHandler PropertyChanged;

		public RelayCommand BackCommand { get; set; }
		public RelayCommand NextCommand { get; set; }
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
	}
}
