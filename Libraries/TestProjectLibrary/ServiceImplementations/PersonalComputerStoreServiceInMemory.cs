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
	/// Implementation of <see cref="IStoreService{TModel}"/> for <see cref="PersonalComputer"/> with in-memory store
	/// </summary>
	public class PersonalComputerStoreServiceInMemory : IStoreService<PersonalComputer>
	{

		/// <summary>
		/// New instance of the class
		/// </summary>
		public PersonalComputerStoreServiceInMemory()
		{
			this.NewID = this.Store.IsEmpty ? 1 : this.Store.Keys.Max() + 1;
		}

		#region Properties

		private ConcurrentDictionary<int, PersonalComputer> Store = new ConcurrentDictionary<int, PersonalComputer>(new Dictionary<int, PersonalComputer>()
		{
			{ 996, new PersonalComputer() { PersonalComputerID = 996, } },
			{ 997, new PersonalComputer() { PersonalComputerID = 997, } },
			{ 998, new PersonalComputer() { PersonalComputerID = 998, } },
			{ 999, new PersonalComputer() { PersonalComputerID = 999, } },
		});


		/// <inheritdoc cref="LockingTypes"/>
		public LockingTypes LockingType { get; private set; }

		private int NewID;
		private object NewIDLocker = new();

		#endregion

		#region IPersonalComputerStoreService

		/// <inheritdoc cref="IStoreService{TModel}.GetList" />
		public IEnumerable<PersonalComputer> GetList()
		{
			var orderedValues = this.Store.Values.OrderBy(PersonalComputer => PersonalComputer.PersonalComputerID);
			return orderedValues;
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryCreateOrUpdate" />
		public bool TryCreateOrUpdate(PersonalComputer model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			if (!model.TryValidateModel(out var validationErrors))
			{
				error = new ErrorResponse().SetValidationErrors(validationErrors);
				return false;
			}

			// Se l'id non viene compilato si procede con l'inserimento di un nuovo veicolo
			if (model.PersonalComputerID == 0)
			{
				//generazione ID fittizia
				while (true)
				{
					// Genera un nuovo ID
					var newID = this.GenerateID();
					model.PersonalComputerID = newID;

					// Tenta l'inserimento
					if (this.Store.TryAdd(newID, model))
					{
						error = default;
						return true;
					}
				}
			}
			// Aggiornamento veicolo esistente
			throw new NotImplementedException("TODO: implementare");
			//if (this.Store.TryGetValue(model.PersonalComputerID, out var existingVehicle))
			//{
			//	existingVehicle.VehicleType = model.VehicleType;
			//	existingVehicle.VehicleYearOfProduction = model.VehicleYearOfProduction;
			//	existingVehicle.VehicleisActive = model.VehicleisActive;

			//	if (model.VehicleType == VehicleTypes.Car || model.VehicleType == VehicleTypes.Truck)
			//	{
			//		existingVehicle.VehicleKm = model.VehicleKm;
			//		existingVehicle.VehicleWorkingHours = null;
			//	}

			//	if (model.VehicleType == VehicleTypes.Cruise || model.VehicleType == VehicleTypes.Tractor)
			//	{
			//		existingVehicle.VehicleWorkingHours = model.VehicleWorkingHours;
			//		existingVehicle.VehicleKm = null;
			//	}
			//	error = default;
			//	return true;
			//}
			//else
			//{
			//	error = new ErrorResponse().SetNotFound(string.Format(PersonalComputerLoc.NotFoundMessageFormat, model.PersonalComputerID));
			//	return false;
			//}
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryDelete" />
		public bool TryDelete(int id)
		{
			return this.Store.TryRemove(id, out _);
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryRead" />
		public bool TryRead(int id, [NotNullWhen(true)] out PersonalComputer? model)
		{
			return this.Store.TryGetValue(id, out model);
		}

		/// <inheritdoc cref="IStoreService{TModel}.ResetAndSetLocking"/>
		public void ResetAndSetLocking(LockingTypes lockingType)
		{
			this.Store.Clear();
			this.LockingType = lockingType;
			this.NewID = 0;
		}
		#endregion

		#region Metodi private

		private int GenerateID()
		{
			switch (this.LockingType)
			{
				case LockingTypes.NoLock:
					return ++this.NewID;
				case LockingTypes.Interlocked:
					//Nessun altro thread può interferire mentre il valore viene letto, incrementato e riscritto.
					//L’intera operazione è eseguita in una singola istruzione a basso livello, in modo sicuro e veloce.
					return Interlocked.Increment(ref this.NewID);
				case LockingTypes.Lock:
					lock (this.NewIDLocker)
					{
						return ++this.NewID;
					}
				default:
					throw new NotSupportedException(LockingType.ToString());
			}
		}

		#endregion

	}

}
