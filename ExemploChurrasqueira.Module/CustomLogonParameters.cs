using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Module
{
    [DomainComponent, Serializable]
    [DisplayName("Churrasqueira")]
    public class CustomLogonParameters : INotifyPropertyChanged, ISerializable
    {
        private ApplicationUser app_User;
        private string password;
        IObjectSpace objectSpace;

        [RuleRequiredField]
        [XafDisplayName("Usuário")]
        public string UserName { get; set; }


        [DisplayName("Senha")]
        [PasswordPropertyText(true)]
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value) return;
                password = value;
            }
        }
        public CustomLogonParameters() { }
        // ISerializable 
        public CustomLogonParameters(SerializationInfo info, StreamingContext context)
        {
            if (info.MemberCount > 0)
            {
                UserName = info.GetString("UserName");
                Password = info.GetString("Password");
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [System.Security.SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("UserName", UserName);
            info.AddValue("Password", Password);
        }
        public void RefreshPersistentObjects(IObjectSpace objectSpace, IObjectSpace nonSecuredObjectSpace)
        {
            this.objectSpace = nonSecuredObjectSpace;
            ////Descomentar para voltar com Lookup de Usuarios
            //App = (UserName == null) ? null : objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == UserName);

        }
    }
}
