using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.IncludeScopes = false;
    options.SingleLine = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
builder.Services.AddSingleton<IProviderRepository, InMemoryProviderRepository>();
builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Demo Scheduler API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Medical Demo Scheduler API Docs";
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
