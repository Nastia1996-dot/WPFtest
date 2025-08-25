using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for Properties.xaml
	/// </summary>
	public partial class Properties : Window
	{
		public Properties()
		{
			InitializeComponent();
		}

		public bool isDark = (bool)(Application.Current.Properties["DarkMode"] ?? false);


		private void btnDarkMode_Click(object sender, RoutedEventArgs e)
		{
			this.isDark = true;
			this.Background = new SolidColorBrush(Colors.Black);
			this.Foreground = new SolidColorBrush(Colors.LightBlue);

			CambiaColoreTesto(this, Colors.LightBlue);
		}

		private void CambiaColoreTesto(DependencyObject parent, Color coloreTesto)
		{
			int count = VisualTreeHelper.GetChildrenCount(parent);

			for (int i = 0; i < count; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);

				if (child is Label lbl)
				{
					lbl.Foreground = new SolidColorBrush(coloreTesto);
				}
				if(child is Button btn)
				{
					btn.Foreground = new SolidColorBrush(Colors.Black);
				}
				// Ricorsione
				CambiaColoreTesto(child, coloreTesto);
			}
		}
		private void btnLightMode_Click(object sender, RoutedEventArgs e)
		{
			if (this.isDark)
			{
				Application.Current.Properties["LightMode"] = true;
				this.Background = new SolidColorBrush(Colors.White);
				this.Foreground = new SolidColorBrush(Colors.Black);
			}
			else
			{
				return;
			}
		}


		private void btnLoginTime_Click(object sender, RoutedEventArgs e)
		{
			try
			{

			DateTime login = (DateTime)Application.Current.Properties["LoginTime"];
			TimeSpan durata = DateTime.Now - login;
			MessageBox.Show($"Sei dentro da {durata.Minutes} minuti");
			}
			catch
			{
				MessageBox.Show("Impossibile visualizzare il login time");
			}

		}

		private void btnCerca_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog
			{
				DefaultExt = ".txt",
				Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (dlg.ShowDialog() == true)
			{
				var path = dlg.FileName;
				this.txtInput.Text = path;
			}
		}

		private void btnSalva_Click(object sender, RoutedEventArgs e)
		{
			//crea la finestra di dialogo per il salvataggio 
			var dlg = new SaveFileDialog
			{
				DefaultExt = ".txt",
				Filter = "Text documents (*.txt)|*.txt"
			};
			//mostra la finestra e salva solo se l'utente conferma
			if(dlg.ShowDialog() == true)
			{
				string filePath = dlg.FileName;

				try
				{
					//salva il contenuto della textbox nel file scelto
					File.WriteAllText(filePath, this.txtInput2.Text);
					MessageBox.Show(this, "File salvato con successo!");
				}
				catch(IOException ex)
				{
					MessageBox.Show($"Errore durante il salvataggio: {ex.Message}");
				}
			}
		}

		private void btnStampa_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new PrintDialog();
			if(dlg.ShowDialog() == true)
			{
				//Quello che viene stampato è esattamente come si vede a schermo,
				//comprese le decorazioni, bordi, testo, colore di sfondo ecc.
				dlg.PrintVisual(this.txtInput3, "Stampa");
			}
		}

		private void btnStampaDoc_Click(object sender, RoutedEventArgs e)
		{
			string testoDaStampare = txtInput3.Text;

			if(string.IsNullOrEmpty(testoDaStampare) )
			{
				MessageBox.Show("Non c'è nulla da stampre");
			}

			FlowDocument doc = CreaDocumentoDaStampare(testoDaStampare);

			PrintDialog dlg = new PrintDialog();
			if(dlg.ShowDialog() == true)
			{
				IDocumentPaginatorSource idpSource = doc;
				doc.PageHeight = dlg.PrintableAreaHeight;
				doc.PageWidth = dlg.PrintableAreaWidth;
				doc.PagePadding = new Thickness(50);
				doc.ColumnWidth = double.PositiveInfinity;

				dlg.PrintDocument(idpSource.DocumentPaginator, "Stampa del contenuto");
			}
		}

		private FlowDocument CreaDocumentoDaStampare(string contenuto)
		{
			FlowDocument doc = new FlowDocument();

			Paragraph paragraph = new Paragraph(new Run(contenuto));
			paragraph.FontSize = 12;
			paragraph.FontFamily = new FontFamily("Times New Roman");
			paragraph.Margin = new Thickness(20);

			doc.Blocks.Add(paragraph);

			return doc;
		}

		private void btnCerca1_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFolderDialog
			{
				Title = "Select folder",
				Multiselect = false,
			};
			if (dlg.ShowDialog() == true)
			{
				var folder = dlg.FolderName;
				this.txtInput0.Text = folder;
			}
		}
	}
}
