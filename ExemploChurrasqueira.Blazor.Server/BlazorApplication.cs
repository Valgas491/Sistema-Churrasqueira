using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ExemploChurrasqueira.Blazor.Server.Services;
using ExemploChurrasqueira.Blazor.Server.Templates;
using ExemploChurrasqueira.Module;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.Extensions.Configuration;

namespace ExemploChurrasqueira.Blazor.Server
{
    public class ExemploChurrasqueiraBlazorApplication : BlazorApplication
    {
        public ExemploChurrasqueiraBlazorApplication()
        {
            ApplicationName = "ExemploChurrasqueira";
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;
            DatabaseVersionMismatch += ExemploChurrasqueiraBlazorApplication_DatabaseVersionMismatch;
            this.CreateCustomLogonWindowObjectSpace += application_CreateCustomLogonWindowObjectSpace;
        }

        private void application_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e)
        {
            var objectSpace = (SecuredObjectSpaceProvider)((XafApplication)sender).ObjectSpaceProviders.First();
            var nonSecuredObjectSpace = objectSpace.CreateNonsecuredObjectSpace();
            ((CustomLogonParameters)e.LogonParameters).RefreshPersistentObjects(e.ObjectSpace, nonSecuredObjectSpace);
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            IXpoDataStoreProvider dataStoreProvider = GetDataStoreProvider(args.ConnectionString, args.Connection);
            args.ObjectSpaceProviders.Add(new SecuredObjectSpaceProvider((ISelectDataSecurityProvider)Security, dataStoreProvider, true));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
            ((SecuredObjectSpaceProvider)args.ObjectSpaceProviders[0]).AllowICommandChannelDoWithSecurityContext = true;

            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, System.Data.IDbConnection connection)
        {
            XpoDataStoreProviderAccessor accessor = ServiceProvider.GetRequiredService<XpoDataStoreProviderAccessor>();
            lock (accessor)
            {
                if (accessor.DataStoreProvider == null)
                {
                    accessor.DataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, true);
                }
            }
            return accessor.DataStoreProvider;
        }

        protected override void OnSetupStarted()
        {
            base.OnSetupStarted();
            IConfiguration configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            if (configuration.GetConnectionString("ConnectionString") != null)
            {
                ConnectionString = configuration.GetConnectionString("ConnectionString");
            }
            if (System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
            {
                DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
        }

        protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context)
        {
            if (context == TemplateContext.LogonWindow)
                return new Login();
            return base.CreateDefaultTemplate(context);
        }

        private void ExemploChurrasqueiraBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
        {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached)
            {
                e.Updater.Update();
                e.Handled = true;
            }
            else
            {
                string message = "The application cannot connect to the specified database, " +
                    "because the database doesn't exist, its version is older " +
                    "than that of the application or its schema does not match " +
                    "the ORM data model structure. To avoid this error, use one " +
                    "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null)
                {
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }
    }
}
