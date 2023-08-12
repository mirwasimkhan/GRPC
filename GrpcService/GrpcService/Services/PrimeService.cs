using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Forms;

namespace GrpcService.Services
{
	public class PrimeServiceImpl : PrimeService.PrimeServiceBase
	{
		private static readonly List<long> validatedPrimes = new List<long>();
		private const int MaxValidatedPrimes = 10;
		private static long totalMessageCout = 0;

		private bool IsPrime(long n)
		{
			if (n <= 1)
				return false;
			if (n <= 3)
				return true;
			if (n % 2 == 0 || n % 3 == 0)
				return false;
			int i = 5;
			while (i * i <= n)
			{
				if (n % i == 0 || n % (i + 2) == 0)
					return false;
				i += 6;
			}
			return true;
		}

		public override Task<PrimeResponse> CheckPrime(PrimeRequest request, ServerCallContext context)
		{
			var startTime = DateTime.UtcNow;
			totalMessageCout++;
			bool isPrime = IsPrime(request.Number);
			if (isPrime)
			{
				lock (validatedPrimes)
				{
					validatedPrimes.Add(request.Number);
					validatedPrimes.Sort((a, b) => b.CompareTo(a));
					if (validatedPrimes.Count > MaxValidatedPrimes)
						validatedPrimes.RemoveAt(MaxValidatedPrimes);
				}
			}
			return Task.FromResult(new PrimeResponse { IsPrime = isPrime, Number = request.Number});
		}

		public static void DisplayTopValidatedPrimes()
		{
			lock (validatedPrimes)
			{
				Console.WriteLine($"Top {validatedPrimes.Count} highest requested/validated prime numbers: {string.Join(", ", validatedPrimes)} \nCurrent Message Count: {totalMessageCout}");
			}
		}

		public override async Task<MultiPrimeResponse> CheckPrimeStream(IAsyncStreamReader<PrimeRequest> requestStream, ServerCallContext context)
		{
			var response = new MultiPrimeResponse
			{
				MultiPrime = { }
			};

			await foreach(var request in requestStream.ReadAllAsync())
			{
				var isPrime = await CheckPrime(request, context);
				response.MultiPrime.Add(isPrime);
			}

			return response;
		}
	}


}
