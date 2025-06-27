using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models.Enums
{
	/// <summary>
	/// Types of company vehicles.
	/// 
	/// - `Car`: car
	/// - `Truck`: truck
	/// - `Cruise`: cruise
	/// - `Tractor`: tractor
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum VehicleTypes
	{
		/// <summary>
		/// Car
		/// </summary>
		[EnumMember(Value = "0")]
		Car,

		/// <summary>
		/// Truck
		/// </summary>
		[EnumMember(Value = "1")]
		Truck,

		/// <summary>
		/// Cruise
		/// </summary>
		[EnumMember(Value = "2")]
		Cruise,

		/// <summary>
		/// Tractor
		/// </summary>
		[EnumMember(Value = "3")]
		Tractor
	}


}
