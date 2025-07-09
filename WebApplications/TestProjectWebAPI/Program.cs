
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using TestProjectLibrary.Models;
using TestProjectLibrary.ServiceImplementations;
using TestProjectLibrary.Services;

namespace TestProjectWebAPI
{
	/// <summary>
	/// Entry point for the Web API application.
	/// </summary>
	public class Program
	{

		private static bool UseInMemoryStore = true;

		/// <summary>
		/// Main method of the application
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers()
				 .AddJsonOptions(options =>
				 {
					 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				 });
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.IncludeXmlComments(Path.ChangeExtension(typeof(CompanyVehicle).Assembly.Location, ".xml"));
				options.IncludeXmlComments(Path.ChangeExtension(typeof(Program).Assembly.Location, ".xml"));
			});
			if (UseInMemoryStore)
			{
				builder.Services.AddSingleton<ICompanyVehicleStoreService, CompanyVehicleStoreServiceInMemory>();
			}
			else
			{
				builder.Services.AddScoped<ICompanyVehicleStoreService, CompanyVehicleStoreServiceOnDb>();
			}

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}

	}
}
