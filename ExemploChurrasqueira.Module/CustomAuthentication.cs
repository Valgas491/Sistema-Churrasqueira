using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

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
            
            if (string.IsNullOrEmpty(customLogonParameters.UserName))
                customLogonParameters.UserName = "Admin";
            ApplicationUser AppUser = objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == customLogonParameters.UserName);

            if (AppUser == null)
                throw new AuthenticationException(
                    customLogonParameters.UserName, "Usuário não encontrado.");

            if (!AppUser.ComparePassword(customLogonParameters.Password))
                throw new AuthenticationException(
                    AppUser.UserName, "Senha incorreta.");

            objectSpace.CommitChanges();
            return AppUser;
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
