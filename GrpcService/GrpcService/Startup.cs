using GrpcService.Services;

namespace GrpcService
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGrpc(options => options.EnableDetailedErrors = true);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment web)
		{
			if (web.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<PrimeServiceImpl>();
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
				});
			});

			app.Build();

		}
	}
}

//using Grpc.Core;
//using GrpcService;
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
