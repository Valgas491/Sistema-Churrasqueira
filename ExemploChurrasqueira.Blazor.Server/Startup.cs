﻿using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using ExemploChurrasqueira.Blazor.Server.Services;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace ExemploChurrasqueira.Blazor.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));
        services.AddDevExpressBlazor();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpContextAccessor();
        services.AddSingleton<XpoDataStoreProviderAccessor>();
        services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
        services.AddScoped<ReservaSchedulerModel>();
        //services.AddXaf<ExemploChurrasqueiraBlazorApplication>(Configuration);
        services.AddXaf(Configuration, builder =>
        {
            builder.UseApplication<ExemploChurrasqueiraBlazorApplication>();
            builder.Modules
                .AddConditionalAppearance()
                .AddReports(options => {
                    options.EnableInplaceReports = true;
                    options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
                    options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
                })
                .AddValidation(options =>
                {
                    options.AllowValidationDetailsAccess = false;
                })
                .Add<ExemploChurrasqueira.Module.ExemploChurrasqueiraModule>()
                .Add<ExemploChurrasqueiraBlazorModule>();
            builder.ObjectSpaceProviders
                .AddXpo((serviceProvider, options) =>
                {
                    string connectionString = null;
                    if (Configuration.GetConnectionString("ConnectionString") != null)
                    {
                        connectionString = Configuration.GetConnectionString("ConnectionString");
                    }

                    ArgumentNullException.ThrowIfNull(connectionString);
                    options.ConnectionString = connectionString;
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
                .AddNonPersistent();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseXaf();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapXafEndpoints();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
    }
}
