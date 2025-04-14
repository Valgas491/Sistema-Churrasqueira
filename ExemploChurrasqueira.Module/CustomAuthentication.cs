using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using System.DirectoryServices.AccountManagement;

namespace ExemploChurrasqueira.Module
{
    public class CustomAuthentication : AuthenticationBase, IAuthenticationStandard
    {
        private CustomLogonParameters customLogonParameters;
        public CustomAuthentication()
        {
            customLogonParameters = new CustomLogonParameters();
        }


        public override void Logoff()
        {
            base.Logoff();
            customLogonParameters = new CustomLogonParameters();
        }

        public override void ClearSecuredLogonParameters()
        {
            customLogonParameters.Password = "";
            base.ClearSecuredLogonParameters();
        }

        public override object Authenticate(IObjectSpace objectSpace)
        {

            if (string.IsNullOrEmpty(customLogonParameters.UserName) || string.IsNullOrEmpty(customLogonParameters.Password))
                throw new AuthenticationException("Usuário ou senha inválidos.");

            Parametros parametro = objectSpace.GetObjects<Parametros>().FirstOrDefault();

            if (parametro is null)
                throw new UserFriendlyException("Configurações de parâmetros inválidas.");

            using (var context = new PrincipalContext(ContextType.Domain, parametro.Dominio))
            {
                bool usuarioAutenticado = context.ValidateCredentials(customLogonParameters.UserName, customLogonParameters.Password);

                if (!usuarioAutenticado)
                    throw new AuthenticationException("Usuário ou senha inválidos.");
            }

            ApplicationUser appUser = objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == customLogonParameters.UserName);

            if (appUser is null)
            {
                appUser = objectSpace.CreateObject<ApplicationUser>();
                appUser.UserName = customLogonParameters.UserName;
                appUser.SetPassword(customLogonParameters.Password);
            }

            objectSpace.CommitChanges();
            return appUser;
        }
        public override void SetLogonParameters(object logonParameters)
        {
            customLogonParameters = (CustomLogonParameters)logonParameters;
            
        }

        public override IList<Type> GetBusinessClasses()
        {
            return new Type[] { typeof(CustomLogonParameters) };
        }
        public override bool AskLogonParametersViaUI
        {
            get { return true; }
        }
        public override object LogonParameters
        {
            get { return customLogonParameters; }

        }
        public override bool IsLogoffEnabled
        {
            get { return true; }
        }
    }
}
