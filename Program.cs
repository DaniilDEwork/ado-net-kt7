using Kt7Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<AppStore>();
var app = builder.Build();
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = "Не удалось обработать запрос" });
}));
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();
