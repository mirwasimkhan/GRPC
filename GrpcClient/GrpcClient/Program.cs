using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcClient;
using GrpcService;

namespace GrpcClient
{
	public class Program
	{
		static async Task Main(string[] args)
		{

			// The port number must match the port of the gRPC server.
			using var channel = GrpcChannel.ForAddress("http://localhost:7046");
			var client = new PrimeService.PrimeServiceClient(channel);
			//var reply = await client.CheckPrimeAsync(
			//				  new PrimeRequest { Id=1,Number=10,TimeStamp=2 });
			//Console.WriteLine("Is Number Prime: " + reply.IsPrime);
			//Console.WriteLine("Press any key to exit...");
			//Console.ReadKey();

			const int NumberOfRequests = 10000;

			
			for (int i = 0; i < NumberOfRequests; i++)
			{
				int randomNum = new Random().Next(1, 1001);
				var request = new PrimeRequest { Number = randomNum };
				var startTime = DateTime.UtcNow;
				var response = await client.CheckPrimeAsync(request);
				var rtt = (DateTime.UtcNow - startTime).TotalMilliseconds;
				Console.WriteLine($"Request, RTT: {rtt} ms");
			}

			Console.WriteLine("All requests sent.");
		}
	}
}