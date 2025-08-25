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
    /// Interaction logic for DettagliPage.xaml
    /// </summary>
    public partial class DettagliPage : Page
    {
		private Gatto _gatto;
        public DettagliPage(Gatto gatto)
        {
            InitializeComponent();
			this._gatto = gatto;	

			this.txtNome.Text = _gatto.Nome;
			this.txtEta.Text = _gatto.Eta.ToString();
			this.txtDescrizione.Text = _gatto.Descrizione;

			this.imgGatto.Source = new BitmapImage(new Uri(gatto.ImmaginePath, UriKind.RelativeOrAbsolute));
		}

		
		public void btnCalcolaEta_Click(object sender, RoutedEventArgs e)
		{
			 int etaUmana = _gatto.Eta * 7;
			this.txtCalcoloEta.Text = $"L'età umana è circa: {etaUmana} anni";
		}
	}
}
