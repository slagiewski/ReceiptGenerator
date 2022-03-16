using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using ReceiptGenerator.Application;
using ReceiptGenerator.Application.Common.Interfaces;
using ReceiptGenerator.Infrastructure;
using ReceiptGenerator.Infrastructure.Persistence;
using ReceiptGenerator.WebUI.Services;
using WebUI.Areas.Identity;
using WebUI.Data;
using ReceiptGenerator.WebUI.Filters;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using ReceiptGenerator.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>(); //TODO

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ReceiptGenerator.Infrastructure.Persistence.ApplicationDbContext>();

builder.Services.AddControllersWithViews(options =>
    options.Filters.Add<ApiExceptionFilterAttribute>())
        .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

builder.Services.AddRazorPages();

builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

builder.Services.AddServerSideBlazor();

builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "ReceiptGenerator API";
    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSwaggerUi3(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.UseRouting();

app.UseAuthentication();
//app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ReceiptGenerator.Infrastructure.Persistence.ApplicationDbContext>();

        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
        await ApplicationDbContextSeed.SeedSampleDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}


await app.RunAsync();


