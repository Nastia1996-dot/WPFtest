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
		NoLock,

		/// <summary>
		/// Interlocked.Increment
		/// </summary>
		Interlocked,

		/// <summary>
		/// lock()
		/// </summary>
		Lock,

	}

}
