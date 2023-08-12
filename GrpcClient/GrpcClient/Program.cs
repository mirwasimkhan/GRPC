using System.Diagnostics;
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
			var missingNumbers = new List<long>();
			do
			{
				missingNumbers = await ExecuteServiceCalls();

			} while (missingNumbers.Count() > 0);
			
			Console.WriteLine("\nAll requests sent.\n\n");
		}

		private static async Task<List<long>> ExecuteServiceCalls()
		{
			//Number of requests
			const int NumberOfRequests = 10000;
			
			//Creating channel from port
			using var channel = GrpcChannel.ForAddress("http://localhost:7046");
			var client = new PrimeService.PrimeServiceClient(channel);						
			
			//hashmap for generating RTT
			Dictionary<long, long> rttlst = new Dictionary<long, long>();
			var sentNumbers = new List<long>();
			
			//initializing client stream to send data
			using var streamCall = client.CheckPrimeStream();

			//Sending requests in a stream to server and calculating RTT
			foreach (var i in Enumerable.Range(1, NumberOfRequests))
			{
				int randomNum = new Random().Next(0, NumberOfRequests);
				sentNumbers.Add(randomNum);
				var request = new PrimeRequest { Number = randomNum };
				var startTime = DateTime.UtcNow;
				await streamCall.RequestStream.WriteAsync(request);
				rttlst[randomNum] = (DateTime.UtcNow - startTime).Milliseconds;				
			}

			//Letting the server know all data has been sent - complete stream and send response.
			await streamCall.RequestStream.CompleteAsync();
			var response = await streamCall;

			foreach (var res in response.MultiPrime)
			{
				Console.WriteLine($"\nNumber: {res.Number}, Is prime: {res.IsPrime}, RTT: {rttlst[res.Number]}");
			}
			
			//Searching to find in any numbers went missing
			var missingNumbers = response.MultiPrime.Select(s => s.Number).Except(sentNumbers);
			
			return missingNumbers == null ? new List<long>() :  missingNumbers.ToList();

		}
	}
}