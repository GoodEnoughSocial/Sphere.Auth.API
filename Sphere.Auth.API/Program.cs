using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Sphere.Auth.API;
using Sphere.Auth.API.Data;
using Sphere.Auth.API.Models;
using Sphere.Shared;


Log.Logger = SphericalLogger.SetupLogger();

Log.Information("Starting up");

var registration = Services.Auth.GetServiceRegistration();

try
{
    var result = await Services.RegisterService(registration);

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(SphericalLogger.ConfigureLogger);

    builder.Services.AddRazorPages();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
    const string connectionString = @"Data Source=Duende.IdentityServer.Quickstart.EntityFramework.db";

    builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

        // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
        options.EmitStaticAudienceClaim = true;
    })
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlite(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlite(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddServerSideSessions()
    .AddAspNetIdentity<ApplicationUser>();

    builder.Services.AddAuthentication();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (args.Contains("/seed"))
    {
        Log.Information("Seeding database...");
        SeedData.EnsureSeedData(app);
        Log.Information("Done seeding database. Exiting.");
        return;
    }

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // InitializeDatabase(app);

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseRouting();

    app.UseIdentityServer();

    app.UseAuthorization();

    app.MapRazorPages().RequireAuthorization();

    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name != "StopTheHostException")
    {
        Log.Fatal(ex, "Unhandled exception");
    }
}
finally
{
    await Services.UnregisterService(registration);

    Log.Information("Shutting down");
    Log.CloseAndFlush();
}


static void InitializeDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
    
    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    context.Database.Migrate();
    if (!context.Clients.Any())
    {
        foreach (var client in Config.Clients)
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
        foreach (var resource in Config.IdentityResources)
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
        foreach (var resource in Config.ApiScopes)
        {
            context.ApiScopes.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }
}
