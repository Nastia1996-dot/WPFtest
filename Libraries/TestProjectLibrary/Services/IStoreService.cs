using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;

namespace TestProjectLibrary.Services
{

	/// <summary>
	/// Generic model store service
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public interface IStoreService<TModel>
		where TModel : IValidableModel
	{

		#region Methods

		/// <summary>
		/// Try to read a model
		/// </summary>
		/// <param name="id"><inheritdoc cref="IValidableModel.ID" path="/summary"/></param>
		/// <param name="model"><c>out</c>: model data if exists, <c>null</c> otherwise</param>
		/// <returns><c>true</c> if the model exists, <c>false</c> otherwise</returns>
		bool TryRead(int id, [NotNullWhen(true)] out TModel? model);

		/// <summary>
		/// Get the full list of models
		/// </summary>
		/// <returns></returns>
		IEnumerable<TModel> GetList();

		/// <summary>
		/// Create or update a model
		/// </summary>
		/// <param name="model">
		/// Model to create or update.
		/// Based on the value of <see cref="IValidableModel.ID"/>:
		/// <list type="bullet">
		/// <item>If 0 or less: a new model is created</item>
		/// <item>If more than 0: the model with the selected id is updated</item>
		/// </list>
		/// </param>
		/// <param name="error"><c>out</c>: if the return value is <c>false</c>, contains the error details</param>
		/// <returns><c>true</c> if the create or update has been performed successfully, <c>false</c> otherwise</returns>
		bool TryCreateOrUpdate(TModel model, [NotNullWhen(false)] out ErrorResponse? error);

		/// <summary>
		/// Try to delete a model
		/// </summary>
		/// <param name="id"><inheritdoc cref="IValidableModel.ID" path="/summary"/></param>
		/// <returns><c>true</c> if the deletion has been performed successfully, <c>false</c> if the record has not been found</returns>
		bool TryDelete(int id);

		/// <summary>
		/// Reset the models store and set Locking type: no lock, lock or interlocked
		/// </summary>
		/// <returns></returns>
		void ResetAndSetLocking(LockingTypes lockingType);

		#endregion

	}

}
