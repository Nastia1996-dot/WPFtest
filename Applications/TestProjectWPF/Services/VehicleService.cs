using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Models;

namespace TestProjectWPF.Services
{
	public class VehicleService
	{
		private readonly HttpClient _httpClient;

		private readonly TestAPIClient Client;

		public VehicleService()
		{

			this._httpClient = new HttpClient();
			//this._httpClient.BaseAddress = new Uri("https://localhost:7213/vehicle");
			this.Client = new TestAPIClient("https://localhost:7213/", _httpClient);

		}

		public async Task<ICollection<CompanyVehicle>> GetVehiclesAsync()
		{
			return await this.Client.VehicleAllAsync();
			var json = await _httpClient.GetStringAsync("/vehicle");
			return JsonConvert.DeserializeObject<List<CompanyVehicle>>(json);
			//return await _httpClient.GetFromJsonAsync<List<CompanyVehicle>>("/vehicle");

		}

		public async Task<CompanyVehicle> GetVehicleByIDAsync(int vehicleID)
		{
			return await this.Client.VehicleGETAsync(vehicleID);
		}

		public async Task<CompanyVehicle> PostVehicleAsync(CompanyVehicle vehicle, int? vehicleID)
		{
			return await this.Client.VehiclePOSTAsync(vehicle);
		}
	}
}
