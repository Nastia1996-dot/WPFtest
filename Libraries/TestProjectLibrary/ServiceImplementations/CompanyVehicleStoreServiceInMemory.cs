using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using TestProjectLibrary.Services;

namespace TestProjectLibrary.ServiceImplementations
{

	/// <summary>
	/// Implementation of <see cref="ICompanyVehicleStoreService"/> with in-memory store
	/// </summary>
	public class CompanyVehicleStoreServiceInMemory : ICompanyVehicleStoreService
	{

		#region Properties

		private ConcurrentDictionary<int, CompanyVehicle> Store = new ConcurrentDictionary<int, CompanyVehicle>(new Dictionary<int, CompanyVehicle>()
		{
			{ 996, new CompanyVehicle() { VehicleID = 996, VehicleType = VehicleTypes.Car, VehicleYearOfProduction = 2005, VehicleisActive = true, VehicleKm = 12500 } },
			{ 997, new CompanyVehicle() { VehicleID = 997, VehicleType = VehicleTypes.Truck, VehicleYearOfProduction = 1999, VehicleisActive = true, VehicleKm = 100000 } },
			{ 998, new CompanyVehicle() { VehicleID = 998, VehicleType = VehicleTypes.Cruise, VehicleYearOfProduction = 2010, VehicleisActive = false, VehicleWorkingHours = 2500 } },
			{ 999, new CompanyVehicle() { VehicleID = 999, VehicleType = VehicleTypes.Tractor, VehicleYearOfProduction = 1995, VehicleisActive = true, VehicleWorkingHours = 3450 } },
		});


		/// <inheritdoc cref="LockingTypes"/>
		public LockingTypes LockingType { get; private set; }

		private int NewID = 0;

		#endregion

		#region ICompanyVehicleStoreService

		IEnumerable<CompanyVehicle> ICompanyVehicleStoreService.GetList()
		{
			var orderedValues = this.Store.Values.OrderBy(CompanyVehicle => CompanyVehicle.VehicleID);
			return orderedValues;
		}

		bool ICompanyVehicleStoreService.TryCreateOrUpdate(CompanyVehicle model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			error = null;
			// Se l'id non viene compilato si procede con l'inserimento di un nuovo veicolo
			if (model.VehicleID == 0)
			{
				//generazione ID fittizia
				while (true)
				{
					// Genera un nuovo ID
					var newID = this.Store.Any() ? this.Store.Keys.Max() + 1 : 1;
					model.VehicleID = newID;

					// Tenta l'inserimento
					if (this.Store.TryAdd(newID, model))
						return true;
				}
			}
			// Aggiornamento veicolo esistente
			if (this.Store.TryGetValue(model.VehicleID, out var existingVehicle))
			{
				existingVehicle.VehicleType = model.VehicleType;
				existingVehicle.VehicleYearOfProduction = model.VehicleYearOfProduction;
				existingVehicle.VehicleisActive = model.VehicleisActive;

				if (model.VehicleType == VehicleTypes.Car || model.VehicleType == VehicleTypes.Truck)
				{
					existingVehicle.VehicleKm = model.VehicleKm;
					existingVehicle.VehicleWorkingHours = null;
				}

				if (model.VehicleType == VehicleTypes.Cruise || model.VehicleType == VehicleTypes.Tractor)
				{
					existingVehicle.VehicleWorkingHours = model.VehicleWorkingHours;
					existingVehicle.VehicleKm = null;
				}
				return true;
			}
			else
			{
				error = new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat, model.VehicleID));
				return false;
			}
		}

		bool ICompanyVehicleStoreService.TryDelete(int id)
		{
			return this.Store.TryRemove(id, out _);
		}

		bool ICompanyVehicleStoreService.TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? model)
		{
			return this.Store.TryGetValue(id, out model);
		}

		void ICompanyVehicleStoreService.ResetAndSetLocking(LockingTypes lockingType)
		{
			this.Store.Clear();
			this.LockingType = lockingType;
			this.NewID = 0;
		}
		#endregion

	}

}
