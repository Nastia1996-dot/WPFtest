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
	/// Implementatio of <see cref="ICompanyVehicleStoreService"/> on database store
	/// </summary>
	public class CompanyVehicleStoreServiceOnDb : ICompanyVehicleStoreService
	{

		#region ICompanyVehicleStoreService

		/// <inheritdoc cref="ICompanyVehicleStoreService.GetList"/>
		public IEnumerable<CompanyVehicle> GetList()
		{
			throw new NotImplementedException("TODO: GetList");
		}

		/// <inheritdoc cref="ICompanyVehicleStoreService.TryCreateOrUpdate"/>
		public bool TryCreateOrUpdate(CompanyVehicle model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			throw new NotImplementedException("TODO: TryCreateOrUpdate");
		}

		/// <inheritdoc cref="ICompanyVehicleStoreService.TryDelete"/>
		public bool TryDelete(int id)
		{
			throw new NotImplementedException("TODO: TryDelete");
		}

		/// <inheritdoc cref="ICompanyVehicleStoreService.TryRead"/>
		public bool TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? model)
		{
			throw new NotImplementedException("TODO: TryRead");
		}

		/// <inheridoc cref="ICompanyVehicleStoreService.ResetAndSetLocking"/>
		public void ResetAndSetLocking(LockingTypes lockingType)
		{
			throw new NotImplementedException();
		}
		#endregion

	}

}
