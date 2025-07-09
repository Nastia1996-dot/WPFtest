using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using static TestProjectLibrary.Models.Enums.LockingTypes;

namespace TestProjectLibrary.Services
{
	
	/// <summary>
	/// Company Vehicle Store Service
	/// </summary>
	public interface ICompanyVehicleStoreService
	{

		#region Methods

		/// <summary>
		/// Try to read a vehicle
		/// </summary>
		/// <param name="id"><inheritdoc cref="CompanyVehicle.VehicleID" path="/summary"/></param>
		/// <param name="model"><c>out</c>: vehicle data if exists, <c>null</c> otherwise</param>
		/// <returns><c>true</c> if the vehicle exists, <c>false</c> otherwise</returns>
		bool TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? model);

		/// <summary>
		/// Get the full list of company vehicles
		/// </summary>
		/// <returns></returns>
		IEnumerable<CompanyVehicle> GetList();

		/// <summary>
		/// Create or update a vehicle
		/// </summary>
		/// <param name="model">
		/// Vehicle to create or update.
		/// Based on the value of <see cref="CompanyVehicle.VehicleID"/>:
		/// <list type="bullet">
		/// <item>If 0 or less: a new vehicle is created</item>
		/// <item>If more than 0: the vehicle with the selected id is updated</item>
		/// </list>
		/// </param>
		/// <param name="error"><c>out</c>: if the return value is <c>false</c>, contains the error details</param>
		/// <returns><c>true</c> if the create or update has been performed successfully, <c>false</c> otherwise</returns>
		bool TryCreateOrUpdate(CompanyVehicle model, [NotNullWhen(false)] out ErrorResponse? error);

		/// <summary>
		/// Try to delete a vehicle
		/// </summary>
		/// <param name="id"><inheritdoc cref="CompanyVehicle.VehicleID" path="/summary"/></param>
		/// <returns><c>true</c> if the deletion has been performed successfully, <c>false</c> if the record has not been found</returns>
		bool TryDelete(int id);

		/// <summary>
		/// Reset vehicles and set Locking type: no lock, lock or interlocked
		/// </summary>
		/// <returns></returns>
		void ResetAndSetLocking(LockingTypes lockingType);

		#endregion

	}

}
