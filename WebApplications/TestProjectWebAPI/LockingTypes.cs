using System.Runtime.Serialization;

namespace TestProjectWebAPI
{
	
	/// <summary>
	/// Tipi di lock
	/// </summary>
	public enum LockingTypes
	{

		/// <summary>
		/// Nessun lock
		/// </summary>
		[EnumMember(Value = "NoLock")]
		NoLock,

		/// <summary>
		/// Interlocked.Increment
		/// </summary>
		[EnumMember(Value = "Interlocked")]
		Interlocked,

		/// <summary>
		/// lock()
		/// </summary>
		[EnumMember(Value = "Lock")]
		Lock,

	}

}
