using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Components;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using ExemploChurrasqueira.Module.BusinessObjects.NoPer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    [DefaultClassOptions]
    [XafDisplayName("Reservar Churrasqueiras")]
    [ImageName("BO_Scheduler")]
    //[Appearance("ManutencaoCor", Criteria = "IsManutencao = true",
    //BackColor = "LightYellow", FontColor = "Red", Priority = 1)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ReservaChurrasqueiraData : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ReservaChurrasqueiraData(Session session)
            : base(session)
        {
        }

        #region Propriedades

        Churrasqueira churrasqueira;
        GerenciarChurrasqueira gerenciarChurrasqueira;
        string associado;
        string npf;
        long qtdPessoas;
        decimal valor;
        Socio socio;
        DateTime dataReserva_Churrasqueira;
        

        #endregion

        #region Lifecycle

        [NonPersistent]
        [XafDisplayName("Socio")]
        [RuleRequiredField]
        [VisibleInListView(false)]
        public Socio Socio
        {
            get { return socio; }
            set
            {
                if (SetPropertyValue(nameof(Socio), ref socio, value))
                {

                    AtualizarDadosDoSocio();
                }
            }

        }

        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [ModelDefault("DisplayFormatInListView", "{0:dd/MM/yyyy}")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [XafDisplayName("Data da Reserva")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DataReserva_Churrasqueira
        {
            get => dataReserva_Churrasqueira;
            set => SetPropertyValue(nameof(DataReserva_Churrasqueira), ref dataReserva_Churrasqueira, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [XafDisplayName("Data da Reserva")]
        public string DataReservaFormatada => DataReserva_Churrasqueira.ToString("dd/MM/yyyy");


        [XafDisplayName("Quantidade Pessoas")]
        public long QtdPessoas
        {
            get => qtdPessoas;
            set
            {
                if (Churrasqueira != null && value > Churrasqueira.QtdComportada)
                {
                    throw new UserFriendlyException($"A quantidade de pessoas ({value}) não pode ser maior que a capacidade da churrasqueira ({Churrasqueira.QtdComportada}).");
                }
                SetPropertyValue(nameof(QtdPessoas), ref qtdPessoas, value);
            }
        }


        [NonPersistent]
        [Browsable(false)]
        public List<Churrasqueira> ChurrasqueirasDisponiveis { get; set; } = new List<Churrasqueira>();

        [Association("Churrasqueira-ReservaChurrasqueiras")]
        [XafDisplayName("Churrasqueira")]
        [DataSourceProperty(nameof(ChurrasqueirasDisponiveis))]
        [RuleRequiredField(DefaultContexts.Save)]
        public Churrasqueira Churrasqueira
        {
            get => churrasqueira;
            set
            {
                if(SetPropertyValue(nameof(Churrasqueira), ref churrasqueira, value))
                {
                    ValorCHurrasuqueira();
                }
            }
        }

        // Adicionada a associação com gerenciamento de churrasqueira
        [Browsable(false)] // Não exibe na interface
        [Association("ReservaChurrasqueira-GerenciarChurrasqueiras")]
        public GerenciarChurrasqueira GerenciarChurrasqueira
        {
            get => gerenciarChurrasqueira;
            set => SetPropertyValue(nameof(GerenciarChurrasqueira), ref gerenciarChurrasqueira, value);
        }

        // Propriedade para diferenciar reservas de manutenção
        [Browsable(false)] // Não exibe na interface
        public bool IsManutencao
        {
            get => GetPropertyValue<bool>(nameof(IsManutencao));
            set => SetPropertyValue(nameof(IsManutencao), value);
        }

        [VisibleInDetailView(false)]
        public string Associado
        {
            get => associado;
            set => SetPropertyValue(nameof(Associado), ref associado, value);
        }

        [VisibleInDetailView(false)]
        public string Npf
        {
            get => npf;
            set => SetPropertyValue(nameof(Npf), ref npf, value);
        }


        [VisibleInDetailView(false)]
        public decimal Valor
        {
            get => valor;
            set => SetPropertyValue(nameof(Valor),ref valor,value);
        }

        #endregion

        #region Métodos
        private void ValorCHurrasuqueira()
        {
            if(churrasqueira != null)
            {
                valor = Churrasqueira.Preco;
            }
            else
            {
                valor = 0;
            }
        }
        
        private void AtualizarDadosDoSocio()
        {
            if (Socio != null)
            {
                Associado = Socio.Nome; 
                Npf = Socio.Npf;
            }
            else
            {
                Associado = string.Empty;
                Npf = string.Empty;
            }
        }
        protected override void OnSaving()
        {
            base.OnSaving();
            
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        #endregion

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