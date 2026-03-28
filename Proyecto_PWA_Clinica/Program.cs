using Proyecto_PWA_Clinica.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddScoped<IUtilitario, Utilitario>();
builder.Services.AddHttpClient<UsuarioService>();
builder.Services.AddHttpClient<CitaService>();
builder.Services.AddHttpClient<TratamientoService>();
builder.Services.AddHttpClient<DashboardService>();
builder.Services.AddHttpClient<PacienteService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
