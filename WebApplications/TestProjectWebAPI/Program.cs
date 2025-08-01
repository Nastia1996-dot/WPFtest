
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

		private static bool UseInMemoryStore = false;

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
			builder.Services.AddSingleton<IDatabaseConnectionSettings>(new DatabaseConnectionSettings() { ConnectionString = "Provider=SQLNCLI11;Data Source=WS-FFORMENTI\\SQLEXPRESS;Persist Security Info=True;Integrated Security=SSPI;Initial Catalog=TestAnastasia" });
			if (UseInMemoryStore)
			{
				builder.Services.AddSingleton<IStoreService<CompanyVehicle>, CompanyVehicleStoreServiceInMemory>();
				builder.Services.AddSingleton<IStoreService<PersonalComputer>, PersonalComputerStoreServiceInMemory>();
			}
			else
			{
				builder.Services.AddScoped<IStoreService<CompanyVehicle>, CompanyVehicleStoreServiceOnDb>();
				builder.Services.AddScoped<IStoreService<PersonalComputer>, PersonalComputerStoreServiceOnDb>();
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
