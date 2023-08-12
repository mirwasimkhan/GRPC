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
			try
			{
				//here t are ticks = Every tick will send 10,000 requests
				for (int t = 0; t < 3; t++)
				{
					const int NumberOfRequests = 10000;
					List<long> isNumbersPrime = new List<long>();

					//Prepare random numbers here
					foreach (var i in Enumerable.Range(1, NumberOfRequests))
					{
						isNumbersPrime.Add(new Random().Next(0, NumberOfRequests));
					}

					do
					{
						isNumbersPrime = await ExecuteServiceCalls(isNumbersPrime);

					} while (isNumbersPrime.Count() > 0);
				}

				Console.WriteLine("\nAll requests sent.\n\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine("----------------Exception Start----------------");
				Console.WriteLine(ex);
				Console.WriteLine("----------------Exception End----------------");
			}

		}

		private static async Task<List<long>> ExecuteServiceCalls(List<long> isNumberPrime)
		{
			//Creating channel from port
			using var channel = GrpcChannel.ForAddress("http://localhost:7046");
			var client = new PrimeService.PrimeServiceClient(channel);

			//hashmap for generating RTT
			Dictionary<long, long> rttlst = new Dictionary<long, long>();
			var sentNumbers = new List<long>();

			//initializing client stream to send data
			using var streamCall = client.CheckPrimeStream();

			//Sending requests in a stream to server and calculating RTT
			foreach (var number in isNumberPrime)
			{
				sentNumbers.Add(number);
				var request = new PrimeRequest { Number = number };
				var startTime = DateTime.UtcNow;
				await streamCall.RequestStream.WriteAsync(request);
				rttlst[number] = (DateTime.UtcNow - startTime).Milliseconds;
			}

			//Letting the server know all data has been sent - complete stream and send response.
			var streamFetchStartTime = DateTime.UtcNow;
			await streamCall.RequestStream.CompleteAsync();
			var response = await streamCall;
			var streamFetchRTT = (DateTime.UtcNow - streamFetchStartTime).Milliseconds;

			foreach (var res in response.MultiPrime)
			{
				Console.WriteLine($"\nNumber: {res.Number}, Is prime: {res.IsPrime}, RTT: {rttlst[res.Number]}");
			}

			Console.WriteLine($"\n\nStream RTT: {streamFetchRTT}\n\n\n");

			//Searching to find in any numbers went missing
			var missingNumbers = response.MultiPrime.Select(s => s.Number).Except(sentNumbers);

			return missingNumbers == null ? new List<long>() : missingNumbers.ToList();

		}
	}
}