using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Churrasqueira : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Churrasqueira(Session session)
            : base(session)
        {
        }
        string nome;
        decimal preco;
        long qtdComportada;
        bool? padrao;
       
       
        public string Nome
        {
            get => nome;
            set => SetPropertyValue(nameof(Nome), ref nome, value);
        }

        [RuleRange(0, long.MaxValue)]
        [XafDisplayName("Quantidade Máxima de Pessoas")]
        public long QtdComportada
        {
            get => qtdComportada;
            set => SetPropertyValue(nameof(QtdComportada), ref qtdComportada, value);
        }

        [ModelDefault("AllowEdit", "true")]
        [CaptionsForBoolValues("Sim", "Não")]
        [VisibleInListView(false)]
        [XafDisplayName("Utilizar valor padrão da Churrasqueira?")]
        public bool? Padrao
        {
            get => padrao;
            set
            {
                SetPropertyValue(nameof(Padrao), ref padrao, value);
                if (padrao == true)
                {
                    Preco = 50.98m;
                }
                else
                {
                    Preco = 0m;
                }
            }
        }

        [VisibleInListView(false)]
        [XafDisplayName("Valor")]
        public decimal Preco
        {
            get => preco;
            set => SetPropertyValue(nameof(Preco), ref preco, value);
        }


        [Association("Churrasqueira-ReservaChurrasqueiras")]
        [Browsable(false)]
        public XPCollection<ReservaChurrasqueiraData> ReservaChurrasqueiras
        {
            get
            {
                return GetCollection<ReservaChurrasqueiraData>(nameof(ReservaChurrasqueiras));
            }
        }

        [Association("Churrasqueira-GerenciarChurrasqueiras")]
        [Browsable(false)]
        public XPCollection<GerenciarChurrasqueira> gerenciarChurrasqueiras
        {
            get
            {
                return GetCollection<GerenciarChurrasqueira>(nameof(gerenciarChurrasqueiras));
            }
        }
        
        protected override void OnSaving()
        {
            base.OnSaving();

            var existente = Session.FindObject<Churrasqueira>(CriteriaOperator.Parse("Nome == ?", Nome));
            if (existente != null && existente.Oid != Oid)
            {
                throw new UserFriendlyException("Já existe uma churrasqueira com este nome. Por favor, escolha outro nome.");
            }
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new UserFriendlyException("O campo Nome é obrigatório.");
            }
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Preco = 0;
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