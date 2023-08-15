using Grpc.Net.Client;
using GrpcService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceTest.Tests
{


	[TestFixture]
	public class TestAsyncOperations
	{
		private GrpcChannel _channel;
		private PrimeService.PrimeServiceClient _client;

		[SetUp]
		public void Setup()
		{
			_channel = GrpcChannel.ForAddress("http://localhost:7046");
			_client = new PrimeService.PrimeServiceClient(_channel);
		}

		[TearDown]
		public void Teardown()
		{
			// Clean up resources
			_channel.ShutdownAsync().Wait();
		}

		[Test]
		public async Task TestAsyncIsPrimeOperation()
		{
			// Prepare test data or input parameters
			var request = new PrimeRequest
			{
				Id = 1,
				Number = 3,
				TimeStamp = 4
			};

			// Make an asynchronous gRPC call
			var response = await _client.CheckPrimeAsync(request);

			// Perform assertions on the response or other outcomes of the async operation
			Assert.AreEqual(true, response.IsPrime);
		}

		[Test]
		public async Task TestAsyncIsNotPrimeOperation()
		{
			// Prepare test data or input parameters
			var request = new PrimeRequest
			{
				Id = 1,
				Number = 10,
				TimeStamp = 4
			};

			// Make an asynchronous gRPC call
			var response = await _client.CheckPrimeAsync(request);

			// Perform assertions on the response or other outcomes of the async operation
			Assert.AreEqual(false, response.IsPrime);
		}
	}
}



