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
			
			while (true)
			{
				PrimeServiceImpl.DisplayTopValidatedPrimes();
				await Task.Delay(TimeSpan.FromSeconds(1));				
			}
		}
	}
}