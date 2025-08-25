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
	/// Interaction logic for CounterControl.xaml
	/// </summary>
	public partial class CounterControl : UserControl
	{
		public CounterControl()
		{
			this.InitializeComponent();
		}


		public static readonly DependencyProperty CountProperty =
				DependencyProperty.Register(
					nameof(Count), typeof(int), typeof(CounterControl), new PropertyMetadata(0));

		public int Count
		{
			get { return (int)GetValue(CountProperty); }
			set { this.SetValue(CountProperty, value); }
		}
	}
}
