using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifoFifo.Models
{

	public class InvoiceLine
	{

		public string ProductCode { get; set; }

		public int Qty { get; set; }

		public decimal UnitPrice { get; set; }

		public decimal Amount { get => this.Qty * this.UnitPrice; }

	}

}
