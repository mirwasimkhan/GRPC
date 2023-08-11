using System;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcService.Services;
using GrpcService;
using Microsoft.AspNetCore.Hosting.Server;
using Grpc.Core.Interceptors;

namespace GrpcService
{
	class Program
	{
		static async Task Main(string[] args)
		{
			const int Port = 7046;

			Server server = new Server
			{
				Services = { PrimeService.BindService(new PrimeServiceImpl()) },
				Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
			};
			server.Start();

			Console.WriteLine($"Prime server listening on port {Port}");
			//var primeService = server.Services.Where(s => s.GetType().Name == "PrimeServiceImpl") as PrimeServiceImpl;
			//var primeService = server.Services.OfType<PrimeServiceImpl>()?.Single();

			while (true)
			{
				//var primeService = server.Services.FirstOrDefault(s => s is PrimeService.PrimeServiceBase) as PrimeService.PrimeServiceBase;
				//if (primeService != null)
				//{
				//	primeService.DisplayTopValidatedPrimes(); // Call the method to display top validated prime numbers
				//}
				PrimeServiceImpl.DisplayTopValidatedPrimes();
				await Task.Delay(TimeSpan.FromSeconds(1));
				//primeService.DisplayTopValidatedPrimes();
				//await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}
	}

	public static class Globals
	{
		public static List<long> validatedPrimes = new List<long>();
	}
}


//using GrpcService.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Additional configuration is required to successfully run gRPC on macOS.
//// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//// Add services to the container.
//builder.Services.AddGrpc();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//app.MapGrpcService<PrimeServiceImpl>();
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//app.Run();
