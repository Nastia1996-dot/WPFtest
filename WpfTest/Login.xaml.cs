using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
	
		public MainWindow()
		{
			this.InitializeComponent();
		}

		
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string password = PasswordBox.Password;
			if (this.txtUser.Text == "mimmo" && password== "123")
			{
				Application.Current.Properties["User"] = this.txtUser.Text;
				
				MainWindowLogged menu = new MainWindowLogged();
				menu.Show();

				this.Close();
			}
			else {
				MessageBox.Show("Credenziali non valide", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		//chiudi finestra se viene premuto ESC
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			//this.TextInput += (s, e) =>
			//{
			//	MessageBox.Show($"Hai scritto: {e.Text}");
			//};
			if(e.Key == Key.Escape)
			{
				MessageBox.Show("Hai premuto il tasto ESC. La finestra si chiuderà");
				this.Close();
			}
		}

		
		//private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
		//{
		//	this.Title = $"[Preview] Mouse a {e.GetPosition(this)}";
		//}


		//private void Button_Click(object sender, RoutedEventArgs e)
		//{
		//	if(int.TryParse(this.InputBox.Text, out int value))
		//	{
		//		this.MyCounter.Count = value;
		//	}
		//	else
		//	{
		//		MessageBox.Show("Inserisci un numero valido");
		//	}
		//}
		//============================================ESERCIZIO 2============================================
		//private void btnAdd_Click(object sender, RoutedEventArgs e)
		//{
		//	if(!string.IsNullOrEmpty(this.txtName.Text) && !lstNames.Items.Contains(this.txtName.Text))
		//	{
		//		this.lstNames.Items.Add(this.txtName.Text);
		//		this.txtName.Clear();
		//	}
		//}

		//============================================ESERCIZIO 3============================================
	}
}