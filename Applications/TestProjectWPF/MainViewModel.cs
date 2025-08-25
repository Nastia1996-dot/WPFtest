using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectWPF.Services;

namespace TestProjectWPF
{
	internal class MainViewModel
	{
		public ObservableCollection<CompanyVehicle> Vehicles { get; set; } = new();

		private readonly VehicleService Service = new VehicleService();

		public async Task LoadVehicleAsync()
		{
			var data = await this.Service.GetVehiclesAsync();
			this.Vehicles.Clear();
			foreach (var v in data)
			{
				this.Vehicles.Add(v);
			}
		}

		public async Task LoadVehicleByIDAsync(int vehicleID)
		{
			var data = await this.Service.GetVehicleByIDAsync(vehicleID);
			this.Vehicles.Clear();
			this.Vehicles.Add(data);

			//foreach(var v in data)
			//{
			//	if(data.VehicleKm == null)
			//	{
					
			//	}
			//}
		}

		public async Task InsertVehicleAsync(CompanyVehicle vehicle, int? vehicleID)
		{
			await this.Service.PostVehicleAsync(vehicle, vehicleID);

			this.Vehicles.Clear();
			this.Vehicles.Add(vehicle);
		}
	}
}
