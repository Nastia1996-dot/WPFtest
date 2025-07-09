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
	/// Locking Types
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum LockingTypes
	{
		/// <summary>
		/// NoLock
		/// </summary>
		[EnumMember(Value = "0")]
		NoLock,

		/// <summary>
		/// Lock
		/// </summary>
		[EnumMember(Value = "1")]
		Lock,

		/// <summary>
		/// Interlocked
		/// </summary>
		[EnumMember(Value = "2")]
		Interlocked,
	}

}
