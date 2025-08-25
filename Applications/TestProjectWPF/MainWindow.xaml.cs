using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestProjectLibrary.Models;
using TestProjectLibrary.ServiceImplementations;
using TestProjectWPF.Services;
using DependencyProperty = Windows.UI.Xaml.DependencyProperty;

namespace TestProjectWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel = new();
		public MainWindow()
		{
			
			InitializeComponent();
			//DataContext viene assegnata solo una volta durante l'istanza di una finestra, dopo l'InitializeComponent
			this.DataContext = this._viewModel;
		}


		private async void btnLeggi_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				await _viewModel.LoadVehicleAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			//MessageBox.Show("Leggi");
		}

		//private void btnTest_Click(object sender, RoutedEventArgs e)
		//{
		//	this.btnLeggi.Click += (s, e) =>
		//	{
		//		MessageBox.Show("Ciao");
		//	};
		//}

		private async void btnLeggiConID_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int vehicleID = int.Parse(this.txtSearchBox.Text);
				await _viewModel.LoadVehicleByIDAsync(vehicleID);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btnInserisci_Click(object sender, RoutedEventArgs e)
		{
			try
			{
		
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void txtSearchBox_MouseEnter(object sender, MouseEventArgs e)
		{
			this.txtSearchBox.Text = "";
		}

		private void txtSearchBox_MouseLeave(object sender, MouseEventArgs e)
		{
			this.txtSearchBox.Text = "Cerca per ID...";
		}
	}
}