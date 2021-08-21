using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PlotDigitizer.NetFramework
{
	/// <summary>
	/// Interaction logic for WaitWindow.xaml
	/// </summary>
	public partial class ProgressPopup : Window
	{
		public ProgressPopup()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Canceled?.Invoke(this, new EventArgs());
			this.Close();
		}

		public event EventHandler Canceled;
	}
}
