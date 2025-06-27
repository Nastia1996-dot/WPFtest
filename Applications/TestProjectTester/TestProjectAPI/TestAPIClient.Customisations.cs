using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectTester.TestProjectAPI
{
	partial class TestAPIClient
	{
		public virtual CompanyVehicle VehicleGET(int vehicleID)
		{
			return AsyncHelper.RunSync(() => this.VehicleGETAsync(vehicleID));
		}

	}
}
