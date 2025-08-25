using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shell;

namespace WpfTest
{
	public partial class MainWindowLogged : Window
	{
		#region Properties
		ColumnDefinition column1CloneForLayer0;
		ColumnDefinition column2CloneForLayer0;
		ColumnDefinition column2CloneForLayer1;


		#endregion

		public MainWindowLogged()
		{
			InitializeComponent();

			this.column1CloneForLayer0 = new ColumnDefinition { SharedSizeGroup = "column1" };
			this.column2CloneForLayer0 = new ColumnDefinition { SharedSizeGroup = "column2" };
			this.column2CloneForLayer1 = new ColumnDefinition { SharedSizeGroup = "column2" };

			bool isDark = (bool)(Application.Current.Properties["DarkMode"] ?? false);
			Application.Current.Properties["LoginTime"] = DateTime.Now;
			
		}

		private void back_Click(object sender, RoutedEventArgs e)
		{
			new MainWindow().Show();
			this.Close();
		}

		private void pane1Pin_Click(object sender, RoutedEventArgs e)
		{
			if (this.btnpane1.Visibility == Visibility.Collapsed)
				this.UndockPane(1);
			else
				this.DockPane(1);
		}

		private void pane2Pin_Click(object sender, RoutedEventArgs e)
		{
			if (this.btnpane2.Visibility == Visibility.Collapsed)
				this.UndockPane(2);
			else
				this.DockPane(2);
		}

		private void btnpane1_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.btnpane1.Visibility == Visibility.Visible)
			{
				this.layer1.Visibility = Visibility.Visible;
				this.layer2.Visibility = Visibility.Collapsed;
				Grid.SetZIndex(this.layer1, 1);
				Grid.SetZIndex(this.layer2, 0);
			}
		}

		private void btnpane2_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.btnpane2.Visibility == Visibility.Visible)
			{
				this.layer2.Visibility = Visibility.Visible;
				this.layer1.Visibility = Visibility.Collapsed;
				Grid.SetZIndex(this.layer2, 1);
				Grid.SetZIndex(this.layer1, 0);
			}
		}

		private void layer0_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.btnpane1.Visibility == Visibility.Visible)
				this.layer1.Visibility = Visibility.Collapsed;

			if (this.btnpane2.Visibility == Visibility.Visible)
				this.layer2.Visibility = Visibility.Collapsed;
		}

		private void pane1_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.btnpane2.Visibility == Visibility.Visible)
				this.layer2.Visibility = Visibility.Collapsed;
		}

		private void pane2_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.btnpane1.Visibility == Visibility.Visible)
				this.layer1.Visibility = Visibility.Collapsed;
		}
		private void layer1_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.btnpane1.Visibility == Visibility.Visible)
			{
				this.layer1.Visibility = Visibility.Collapsed;
			}
		}

		private void layer2_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.btnpane2.Visibility == Visibility.Visible)
			{
				this.layer2.Visibility = Visibility.Collapsed;
			}
		}

		public void DockPane(int paneNumber)
		{
			if (paneNumber == 1)
			{
				this.btnpane1.Visibility = Visibility.Collapsed;
				this.pane1PinImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pin.png"));

				this.layer0.ColumnDefinitions.Add(this.column1CloneForLayer0);
				if (this.btnpane2.Visibility == Visibility.Collapsed)
					this.layer1.ColumnDefinitions.Add(this.column2CloneForLayer1);
			}
			else if (paneNumber == 2)
			{
				this.btnpane2.Visibility = Visibility.Collapsed;
				this.pane2PinImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pin.png"));

				this.layer0.ColumnDefinitions.Add(column2CloneForLayer0);
				if (this.btnpane1.Visibility == Visibility.Collapsed)
					this.layer1.ColumnDefinitions.Add(this.column2CloneForLayer1);
			}
		}

		public void UndockPane(int paneNumber)
		{
			if (paneNumber == 1)
			{
				this.layer1.Visibility = Visibility.Visible;
				this.btnpane1.Visibility = Visibility.Visible;
				this.pane1PinImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pinHorizontal.png"));

				this.layer0.ColumnDefinitions.Remove(this.column1CloneForLayer0);
				this.layer1.ColumnDefinitions.Remove(this.column2CloneForLayer1);
			}
			else if (paneNumber == 2)
			{
				this.layer2.Visibility = Visibility.Visible;
				this.btnpane2.Visibility = Visibility.Visible;
				this.pane2PinImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pinHorizontal.png"));

				this.layer0.ColumnDefinitions.Remove(this.column2CloneForLayer0);
				this.layer1.ColumnDefinitions.Remove(this.column2CloneForLayer1);
			}
		}

		private void DragDropPage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Drag_Drop window = new Drag_Drop();
			window.Show();

		}

		private void KeyboardEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			KeyboardEvents window = new KeyboardEvents();
			window.Show();

		}

		private void Commands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Properties window = new Properties();
			window.Show();
		}

		private void NavBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var navWindow = new NavWindow();
			navWindow.Show();
			this.Close();
        }
		
		
    }
}
