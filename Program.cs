using MesaDeAyuda.Components;
using MesaDeAyuda.Data;
using MesaDeAyuda.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped<TicketService>();

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
