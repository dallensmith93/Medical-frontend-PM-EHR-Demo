using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;

var builder = WebApplication.CreateBuilder(args);
const string AngularCorsPolicy = "AngularFrontend";

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.IncludeScopes = false;
    options.SingleLine = true;
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
builder.Services.AddSingleton<IProviderRepository, InMemoryProviderRepository>();
builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
builder.Services.AddSingleton<IAppointmentPrerequisiteRepository, InMemoryAppointmentPrerequisiteRepository>();
builder.Services.AddSingleton<IAppointmentChargeRepository, InMemoryAppointmentChargeRepository>();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentPrerequisiteService, AppointmentPrerequisiteService>();
builder.Services.AddScoped<IAppointmentChargeService, AppointmentChargeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Demo Scheduler API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Medical Demo Scheduler API Docs";
});

app.UseHttpsRedirection();
app.UseCors(AngularCorsPolicy);
app.MapControllers();

app.Run();
