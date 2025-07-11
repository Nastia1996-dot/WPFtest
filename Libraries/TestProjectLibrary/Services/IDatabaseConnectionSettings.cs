using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Services
{

	/// <summary>
	/// Database connection settings
	/// </summary>
	public interface IDatabaseConnectionSettings
	{

		#region Properties

		/// <summary>
		/// Connection string
		/// </summary>
		string ConnectionString { get; }

		#endregion

	}

}
