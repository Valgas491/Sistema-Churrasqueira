using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per {
    [ImageName("BO_User")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ApplicationUser : PermissionPolicyUser, IObjectSpaceLink, ISecurityUserWithLoginInfo
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ApplicationUser(Session session)
            : base(session) {
        }
        [Browsable(false)]
        [DevExpress.Xpo.Aggregated, Association("User-LoginInfo")]
        public XPCollection<ApplicationUserLoginInfo> LoginInfo
        {
            get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
        }

        string email;

        
        [Association]
        public XPCollection<ReservaChurrasqueiraData> Reserva
        {
            get { return GetCollection<ReservaChurrasqueiraData>(nameof(Reserva)); }
        }
        public string Email
        {
            get { return email; }
            set { SetPropertyValue(nameof(Email), ref email, value); }
        }

        bool podeVisualizarTodosRegistros;
        public bool PodeVisualizarTodosRegistros
        {
            get => podeVisualizarTodosRegistros;
            set => SetPropertyValue(nameof(PodeVisualizarTodosRegistros), ref podeVisualizarTodosRegistros, value);
        }
        [Association]
        [Browsable(false)]
        public XPCollection<Departamento> Departamentos
        {
            get { return GetCollection<Departamento>(nameof(Departamentos)); }
        }

        IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

        IObjectSpace IObjectSpaceLink.ObjectSpace { get; set; }

        ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey)
        {
            ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
            result.LoginProviderName = loginProviderName;
            result.ProviderUserKey = providerUserKey;
            result.User = this;
            return result;
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
    }
}