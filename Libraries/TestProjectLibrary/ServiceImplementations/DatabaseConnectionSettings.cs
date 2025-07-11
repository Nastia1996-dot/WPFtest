using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Services;

namespace TestProjectLibrary.ServiceImplementations
{

	/// <inheritdoc cref="IDatabaseConnectionSettings"/>
	public class DatabaseConnectionSettings : IDatabaseConnectionSettings
	{

		#region Properties

		/// <inheritdoc cref="IDatabaseConnectionSettings.ConnectionString"/>
		public string ConnectionString { get; set; }

		#endregion

	}

}
