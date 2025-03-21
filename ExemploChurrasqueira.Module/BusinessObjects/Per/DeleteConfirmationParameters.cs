using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    public class DeleteConfirmationParameters : BaseObject
    {
        private string _confirmationMessage;
        private string _password;

        public DeleteConfirmationParameters(Session session) : base(session) { }

        public string ConfirmationMessage
        {
            get => _confirmationMessage;
            set => SetPropertyValue(nameof(ConfirmationMessage), ref _confirmationMessage, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string Password
        {
            get => _password;
            set => SetPropertyValue(nameof(Password), ref _password, value);
        }
    }
}
