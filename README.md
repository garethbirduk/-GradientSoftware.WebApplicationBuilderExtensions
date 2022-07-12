# GradientSoftware.WebApplicationBuilderExtensions
A helper for setting up web applications; currently a template rather than a nuget

# WebApplication usage
	await WebApplication
		.CreateBuilder(args)
		.BuildConfiguration()
		.ConfigureEnvironment()
		.ConfigureServices()
		.ConfigureCustomServices() // this will be for adding specific singletons etc
		.ConfigureLogging()
		.Build()
		.ConfigureApp()
		.LogSections("Logging")
		.RunAsync();

# IHostBuilder usage
	using IServiceScope serviceScope = CreateDefaultBuilder(args)
		.BuildConfiguration()
		.AddService<CloneHeroHelper>()
		.Build()
		.Services
		.CreateScope();
	await serviceScope.ServiceProvider
		.GetRequiredService<CloneHeroHelper>()
		.RunAsync();
