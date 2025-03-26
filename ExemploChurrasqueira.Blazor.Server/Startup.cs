using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ExemploChurrasqueira.Blazor.Server.Services;
using ExemploChurrasqueira.Blazor.Server.Templates;
using ExemploChurrasqueira.Module;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevExpress.ExpressApp.AuditTrail;
using ExemploChurrasqueira.Module.BusinessObjects.NoPer;

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
        services.AddAuditTrailXpoServices(options =>
        {
            options.Events.OnCustomizeAuditTrailSettings = context =>
            {
                ConfigureAuditTrail(context);
            };
        });
        //services.AddXaf<ExemploChurrasqueiraBlazorApplication>(Configuration);
        services.AddXaf(Configuration, builder =>
        {
        
            builder.UseApplication<ExemploChurrasqueiraBlazorApplication>();
            services.AddXafSecurity(options =>
            {
                options.RoleType = typeof(PermissionPolicyRole);
                options.UserType = typeof(PermissionPolicyUser);
                //options.Events.OnSecurityStrategyCreated = securityStrategy => ((SecurityStrategy)securityStrategy).RegisterXPOAdapterProviders();
                options.SupportNavigationPermissionsForTypes = false;


                options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
                options.Events.OnSecurityStrategyCreated = securityStrategy =>
                {
                    ((SecurityStrategy)securityStrategy).RegisterXPOAdapterProviders();
                    ((SecurityStrategy)securityStrategy).AnonymousAllowedTypes.Add(typeof(ApplicationUser));
                };
                options.UserType = typeof(ApplicationUser);
            })
            .AddExternalAuthentication<HttpContextPrincipalProvider>()
            .AddAuthenticationProvider<AuthenticationStandardProviderOptions, CustomAuthenticationStandardProvider>(options =>
            {
                options.IsSupportChangePassword = true;
                options.LogonParametersType = typeof(CustomLogonParameters);
            })
            .AddAuthenticationActiveDirectory(o =>
            {
                o.CreateUserAutomatically = true;
            })
            .AddAuthenticationStandard(options =>
            {
                options.IsSupportChangePassword = true;
            });


            var authentication = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            authentication
                .AddCookie(options =>
                {
                    options.LoginPath = "/LoginPage";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Authentication:Jwt:Issuer"],
                        ValidAudience = Configuration["Authentication:Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"]))
                    };
                });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .RequireXafAuthentication()
                        .Build();
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AuthenticationDisplayName = "Windows";
            });
            builder.Modules
                .AddConditionalAppearance()
                .AddAuditTrailXpo()
                .AddReports(options =>
                {
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
    private void ConfigureAuditTrail(CustomizeAuditTrailSettingsContext context)
    {
        
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
        // Definir a cultura padrão para pt-BR
        var cultureInfo = new System.Globalization.CultureInfo("pt-BR");
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var supportedCultures = new[] { cultureInfo };
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };
        app.UseRequestLocalization(localizationOptions);
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
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
