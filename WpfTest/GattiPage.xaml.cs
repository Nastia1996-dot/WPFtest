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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for Page2.xaml
	/// </summary>
	public partial class GattiPage : Page
	{
		public GattiPage()
		{
			InitializeComponent();
		}

		private void ApriDettaglio(object sender, RoutedEventArgs e)
		{
			Gatto gatto = new Gatto();
			gatto.Eta = 5;
			gatto.Descrizione = "Un gatto curioso e giocherellone";
			gatto.Nome = "Mimmo";
			gatto.ImmaginePath = "/Resources/gatto.jpg";

			this.NavigationService.Navigate(new DettagliPage(gatto));
		}
		private void ApriDettaglio2(object sender, RoutedEventArgs e)
		{
			Gatto gatto = new Gatto();
			gatto.Eta = 3;
			gatto.Descrizione = "Un gatto curioso e giocherellone";
			gatto.Nome = "Floki";
			gatto.ImmaginePath = "/Resources/gatto(1).jpg";

			this.NavigationService.Navigate(new DettagliPage(gatto));
		}

		private void ApriDettaglio3(object sender, RoutedEventArgs e)
		{
			Gatto gatto = new Gatto();
			gatto.Eta = 7;
			gatto.Descrizione = "Un gatto curioso e giocherellone";
			gatto.Nome = "Arturo";
			gatto.ImmaginePath = "/Resources/micio.png";

			this.NavigationService.Navigate(new DettagliPage(gatto));
		}
		
	}
}
