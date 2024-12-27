using CinemaAPI.Configuaration;
using CinemaAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Description = "Nhập token vào đây:",
    Name = "Authorization",
    In = ParameterLocation.Header,


    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "Bearer"
});
    opt.OperationFilter<SecurityRequirementsOperationFilter>();
});
var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettings);
GlobalSettings.IncludeConfig(appSettings.Get<AppSettings>());

builder.Services.ConfigureDbContext(GlobalSettings.AppSettings.Database.DatabaseConfig);
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3030")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Đăng ký công việc
    var updateShowtimeJobKey = new JobKey("UpdateShowtimeStateJob");
    q.AddJob<UpdateShowtimeStateJob>(opts => opts.WithIdentity(updateShowtimeJobKey));

    q.AddTrigger(opts => opts
        .ForJob(updateShowtimeJobKey)
        .WithIdentity("UpdateShowtimeStateTrigger")
        .StartNow()
        .WithCronSchedule("0 0/1 * * * ?") // Chạy mỗi phút
    );

    var cancelUnpaidBillsJobKey = new JobKey("CancelUnpaidBillsJob");
    q.AddJob<BillCleanupJob>(opts => opts.WithIdentity(cancelUnpaidBillsJobKey));

    q.AddTrigger(opts => opts
        .ForJob(cancelUnpaidBillsJobKey)
        .WithIdentity("CancelUnpaidBillsTrigger")
        .StartNow()
        .WithCronSchedule("0 0/1 * * * ?") // Chạy mỗi phút
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, @"resources")),
    RequestPath = new PathString("/resources")
});

app.Run();
