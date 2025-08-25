using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfTest
{
	public partial class Drag_Drop : Window
	{
		private Point startPoint;


		public Drag_Drop()
		{
			this.InitializeComponent();
		}

		// oggetto1
		private void oggetto1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.startPoint = e.GetPosition(null);
		}

		private void oggetto1_MouseMove(object sender, MouseEventArgs e)
		{
			this.Drag(this.txtOggetto1, e);
		}

		// oggetto2
		private void oggetto2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.startPoint = e.GetPosition(null);
		}

		private void oggetto2_MouseMove(object sender, MouseEventArgs e)
		{
			this.Drag(this.txtOggetto2, e);
		}

		private void Drag(TextBlock textBlock, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Point currentPos = e.GetPosition(null);
				Vector diff = this.startPoint - currentPos;

				if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					if (textBlock.Parent is FrameworkElement parent)
					{
						string data = parent.Name;
						DataObject dragData = new DataObject(DataFormats.Text, data);
						DragDrop.DoDragDrop(textBlock, dragData, DragDropEffects.Move);
					}
				}
			}
		}


		private void DropZone_Drop(object sender, DragEventArgs e)
		{
			string? droppedText = e.Data.GetData(DataFormats.Text) as string;

			this.DropZone.Background = Brushes.LightGreen;

			if (this.DropZone.Child is TextBlock textBlock)
			{
				textBlock.Text = $"Hai rilasciato: {droppedText}";

				if (droppedText == "Oggetto1")
					this.Oggetto1.Visibility = Visibility.Collapsed;
				else if (droppedText == "Oggetto2")
					this.Oggetto2.Visibility = Visibility.Collapsed;
			}
		}
		private void DropZone_DragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Move;
			e.Handled = true;
		}
	}
}