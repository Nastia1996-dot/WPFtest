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

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for KeyboardEvents.xaml
	/// </summary>
	public partial class KeyboardEvents : Window
	{
		public KeyboardEvents()
		{
			InitializeComponent();
			this.Loaded += (s, e) => this.Focus(); // assicura focus alla finestra
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			this.KeyDownOutput.Text = $"KeyDown: {e.Key}";

			if (e.Key == Key.Escape)
			{
				this.Close(); // chiude la finestra con ESC
			}

			if(e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
				this.txtOutput.Text = "Salvataggio riuscito!";
			}
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			this.KeyUpOutput.Text = $"KeyUp: {e.Key}";
		}

		private void btnInvio_Click(object sender, RoutedEventArgs e)
		{
			this.txtOutput.Text = "";
		}

		
	}
}



	