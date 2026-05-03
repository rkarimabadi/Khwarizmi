using Khwarizmi.Application.Interfaces;
using Khwarizmi.Application.Services;
using Khwarizmi.Domain.Services;
using Khwarizmi.Infrastructure.Persistence;
using Khwarizmi.Infrastructure.Repositories.Persistence;
using Khwarizmi.Presentation.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<PlayerAppService>();
builder.Services.AddScoped<PuzzleAppService>();
builder.Services.AddScoped<PuzzleGenerator>();
builder.Services.AddScoped<AlgebraicValidator>();
builder.Services.AddScoped<DifficultyEvaluator>();
builder.Services.AddScoped<IPlayerRepository, LocalStoragePlayerRepository>();
builder.Services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

await builder.Build().RunAsync();
