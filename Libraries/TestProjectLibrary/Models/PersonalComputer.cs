using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{

	/// <summary>
	/// TODO: creare le proprietà
	/// </summary>
	public class PersonalComputer : IValidableModel
	{

		#region Properties

		/// <summary>
		/// ID of the personal computer
		/// </summary>
		public int PersonalComputerID { get; set; }

		#endregion

		#region Methods

		/// <inheritdoc cref="IValidableModel.TryValidateModel"/>
		public bool TryValidateModel(out ValidationError[] validationErrors)
		{
			throw new NotImplementedException("TODO: implementare");
		}

		#endregion

		#region IValidableModel

		int IValidableModel.ID
		{
			get => this.PersonalComputerID;
			set => this.PersonalComputerID = value;
		}

		#endregion

	}

}
