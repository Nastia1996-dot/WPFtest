using LifoFifo.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

namespace LifoFifo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			double d = 10;
			double d10 = Math.Pow(d, 10);

			var invoice = new InvoiceHeader();
			var list = new List<InvoiceHeader>();
			list.Add(invoice);
			list[0].InvoiceNumber = "5";
			invoice.InvoiceNumber = "5";

			var foundFile = FindFile(new DirectoryInfo(@"C:\temp\testSearch"), "abc.txt");
			//var f = Directory.EnumerateFiles(@"C:\temp\testSearch", "abc.txt");

			int num = 0;
			ReachNumTen(num);
			Increment(6, 3);
		}

		private void Test()
		{

			var invoiceOriginal = new InvoiceHeader()
			{
				InvoiceNumber = "1",
				InvoiceDate = DateTime.Now,
				CustomerCompanyName = "Acme spa",
				Lines = [
					new InvoiceLine()
					{
						ProductCode = "ABC",
						UnitPrice = 12.5m,
						Qty = 3,
					},
					new InvoiceLine()
					{
						ProductCode = "DEF",
						UnitPrice = 14.75m,
						Qty = 2,
					},
				],
			};


			var invoiceModified = new InvoiceHeader()
			{
				InvoiceNumber = "1/A",
				InvoiceDate = DateTime.Now,
				CustomerCompanyName = "Acme s.p.a.",
				Lines = [
					new InvoiceLine()
					{
						ProductCode = "ABC",
						UnitPrice = 12.9m,
						Qty = 3,
					},
					new InvoiceLine()
					{
						ProductCode = "DEF",
						UnitPrice = 14.75m,
						Qty = 2,
					},
					new InvoiceLine()
					{
						ProductCode = "GHI",
						UnitPrice = 19.75m,
						Qty = 1,
					},
				],
			};


			var invoiceModified2 = new InvoiceHeader()
			{
				InvoiceNumber = "1/A",
				InvoiceDate = DateTime.Now,
				CustomerCompanyName = "Acme s.p.a.",
				Lines = [
					new InvoiceLine()
					{
						ProductCode = "ABC",
						UnitPrice = 12.9m,
						Qty = 3,
					},
				],
			};

			invoiceOriginal.InvoiceNumber = invoiceModified.InvoiceNumber;
			invoiceOriginal.InvoiceDate = invoiceModified.InvoiceDate;
			invoiceOriginal.CustomerCompanyName = invoiceModified.CustomerCompanyName;

			var currentLines = new Queue<InvoiceLine>(invoiceOriginal.Lines);
			foreach (var newLine in invoiceModified.Lines)
			{
				if (!currentLines.TryDequeue(out var targetLine))
				{
					targetLine = new InvoiceLine();
					invoiceOriginal.Lines.Add(targetLine);
				}
				targetLine.Qty = newLine.Qty;
				targetLine.UnitPrice = newLine.UnitPrice;
				targetLine.ProductCode = newLine.ProductCode;
			}
			while (currentLines.TryDequeue(out var lineToDelete))
			{
				invoiceOriginal.Lines.Remove(lineToDelete);
			}

		}

		private static FileInfo? FindFile(DirectoryInfo directory, string searchPattern)
		{
			var foundFile = directory.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly).FirstOrDefault();
			//condizione per fermare la ricorsione
			if (foundFile != null)
			{
				return foundFile;
			}
			foreach (var subDir in directory.EnumerateDirectories())
			{
				if (FindFile(subDir, searchPattern) is FileInfo foundSubFile)
				{
					return foundSubFile;
				}
			}
			return null;
		}

		//creare una funzione ricorsiva che parte da un numero e lo incrementa fino a 10

		private static string ReachNumTen(int num)
		{
			string found = "Found!";

			if (num == 10)
			{
				return found;
			}
			else
			{
				return ReachNumTen(num + 1);
			}

		}

		private int Increment(int start, int n)
		{
			if (n == 0)
			{
				return start;
			}
			else
			{
				return Increment(start + 1, n - 1);
			}
		}
	}
}