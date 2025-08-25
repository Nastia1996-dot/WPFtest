using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifoFifo.Models
{

	public class InvoiceHeader
	{

		public string InvoiceNumber { get; set; }

		public DateTime InvoiceDate { get; set; }

		public string CustomerCompanyName { get; set; }

		public ObservableCollection<InvoiceLine> Lines { get; set; } = new ObservableCollection<InvoiceLine>();

	}

}
