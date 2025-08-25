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
	/// Interaction logic for NavWindow.xaml
	/// </summary>
	public partial class NavWindow : NavigationWindow
	{
		public NavWindow()
		{
			InitializeComponent();
			Page1 pag = new Page1();
			this.NavigationService.Navigate(pag);
		}
	}
}
