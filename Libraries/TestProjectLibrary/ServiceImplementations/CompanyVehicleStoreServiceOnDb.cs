using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using TestProjectLibrary.Services;

namespace TestProjectLibrary.ServiceImplementations
{

	/// <summary>
	/// Implementation of <see cref="IStoreService{TModel}"/> for <see cref="CompanyVehicle"/> on database store
	/// </summary>
	public class CompanyVehicleStoreServiceOnDb(IDatabaseConnectionSettings databaseConnectionSettings) : IStoreService<CompanyVehicle>
	{

		#region Properties

		private IDatabaseConnectionSettings DatabaseConnectionSettings { get; } = databaseConnectionSettings;

		#endregion

		#region ICompanyVehicleStoreService

		/// <inheritdoc cref="IStoreService{TModel}.GetList"/>
		public IEnumerable<CompanyVehicle> GetList()
		{
			throw new NotImplementedException("TODO: GetList");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryCreateOrUpdate"/>
		public bool TryCreateOrUpdate(CompanyVehicle model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			throw new NotImplementedException("TODO: TryCreateOrUpdate");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryDelete"/>
		public bool TryDelete(int id)
		{
			throw new NotImplementedException("TODO: TryDelete");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryRead"/>
		public bool TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? model)
		{
			throw new NotImplementedException("TODO: TryRead");
		}

		/// <inheritdoc cref="IStoreService{TModel}.ResetAndSetLocking"/>
		public void ResetAndSetLocking(LockingTypes lockingType)
		{
			throw new NotSupportedException();
		}
		#endregion

	}

}
