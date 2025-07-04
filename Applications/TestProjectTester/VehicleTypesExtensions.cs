using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectTester.TestProjectAPI;

namespace TestProjectTester
{
	public static class VehicleTypesExtensions
	{
			public static bool IsKmRequired(this VehicleTypes types)
			{
				switch (types)
				{
					case VehicleTypes.Car:
						return true;
					case VehicleTypes.Truck:
						return true;
					default: return false;
				}
			}

		public static bool IsWorkinHoursRequired(this VehicleTypes types)
		{
			switch (types)
			{
				case VehicleTypes.Cruise:
					return true;
				case VehicleTypes.Tractor:
					return true;
				default: return false;
			}
		}

	}
}
