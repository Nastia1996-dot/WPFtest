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
	/// Implementation of <see cref="IStoreService{TModel}"/> for <see cref="PersonalComputer"/> on database store
	/// </summary>
	public class PersonalComputerStoreServiceOnDb : IStoreService<PersonalComputer>
	{

		#region IPersonalComputerStoreService

		/// <inheritdoc cref="IStoreService{TModel}.GetList"/>
		public IEnumerable<PersonalComputer> GetList()
		{
			throw new NotImplementedException("TODO: GetList");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryCreateOrUpdate"/>
		public bool TryCreateOrUpdate(PersonalComputer model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			throw new NotImplementedException("TODO: TryCreateOrUpdate");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryDelete"/>
		public bool TryDelete(int id)
		{
			throw new NotImplementedException("TODO: TryDelete");
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryRead"/>
		public bool TryRead(int id, [NotNullWhen(true)] out PersonalComputer? model)
		{
			throw new NotImplementedException("TODO: TryRead");
		}

		/// <inheridoc cref="IStoreService{TModel}.ResetAndSetLocking"/>
		public void ResetAndSetLocking(LockingTypes lockingType)
		{
			throw new NotImplementedException();
		}
		#endregion

	}

}
