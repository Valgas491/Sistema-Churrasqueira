using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraCharts;
using ExemploChurrasqueira.Module.Helper;
using ExemploChurrasqueira.Module.Controllers.ListView;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    [XafDisplayName("Gerenciar Manutenção")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class GerenciarChurrasqueira : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public GerenciarChurrasqueira(Session session)
            : base(session)
        {
        }

        Churrasqueira churrasqueira;
        DateTime dataManutencao;
        ulong qtdDias;


        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [ModelDefault("Caption", "Data Início Da Manutenção")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DataManutencao
        {
            get => dataManutencao;
            set => SetPropertyValue(nameof(DataManutencao), ref dataManutencao, value);
        }
        [XafDisplayName("Quantidade De Dias de Manutenção")]
        public ulong QtdDias
        {
            get => qtdDias;
            set => SetPropertyValue(nameof(QtdDias), ref qtdDias, value);
        }

        [NonPersistent]
        [Browsable(false)]
        public List<Churrasqueira> ChurrasqueirasDisponiveis { get; set; } = new List<Churrasqueira>();

        // Associação com a churrasqueira
        [Association("Churrasqueira-GerenciarChurrasqueiras")]
        [RuleRequiredField(DefaultContexts.Save)]
        [DataSourceProperty(nameof(ChurrasqueirasDisponiveis))]
        public Churrasqueira Churrasqueira
        {
            get => churrasqueira;
            set => SetPropertyValue(nameof(Churrasqueira), ref churrasqueira, value);
        }

        [Association("ReservaChurrasqueira-GerenciarChurrasqueiras")]
        [Browsable(false)]
        public XPCollection<ReservaChurrasqueiraData> reservaChurrasqueiras
        {
            get
            {
                return GetCollection<ReservaChurrasqueiraData>(nameof(reservaChurrasqueiras));
            }
        }

        // Status para controle da churrasqueira
        private TaskStatus status;
        public TaskStatus Status
        {
            get => status;
            set => SetPropertyValue(nameof(Status), ref status, value);
        }
        // Enum para o status
        public enum TaskStatus
        {
            [ImageName("State_Validation_Invalid")]
            [XafDisplayName("Em Manutenção")]
            Maintance,
            [XafDisplayName("Finalizado")]
            [ImageName("Action_MarkCompleted")]
            Completed
        }
        // Método auxiliar para verificar se a reserva está sendo apagada
        private bool IsReservaDeleted()
        {
            return IsDeleted || IsDeleted && Churrasqueira == null;
        }
        protected override void OnSaving()
        {
            base.OnSaving();

            if (Churrasqueira != null && DataManutencao > DateTime.MinValue && QtdDias > 0)
            {
                try
                {
                    for (ulong i = 0; i < QtdDias; i++)
                    {
                        DateTime dataReservaAtual = DataManutencao.AddDays(i);

                        // Verifica se a reserva já existe
                        var reservaExistente = Session.FindObject<ReservaChurrasqueiraData>(
                            CriteriaOperator.Parse("Churrasqueira.Oid = ? AND dataReserva_Churrasqueira = ? AND IsManutencao = true",
                            Churrasqueira.Oid, dataReservaAtual));

                        if (reservaExistente == null) // Apenas cria se não existir
                        {
                            var novaReserva = new ReservaChurrasqueiraData(Session)
                            {
                                Churrasqueira = Churrasqueira,
                                DataReserva_Churrasqueira = dataReservaAtual,
                                QtdPessoas = 0,
                                GerenciarChurrasqueira = this,
                                IsManutencao = true,
                                Valor = 0
                            };

                            Session.Save(novaReserva);
                        }
                        else
                        {
                            // Atualiza a reserva existente, se necessário
                            reservaExistente.GerenciarChurrasqueira = this;
                            reservaExistente.IsManutencao = true;
                            Session.Save(reservaExistente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao salvar reservas: {ex.Message}");
                    throw;
                }
            }
            else if (!IsReservaDeleted())
            {
                throw new UserFriendlyException("Você deve selecionar uma Data, uma Churrasqueira e informar a quantidade de dias.");
            }
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            ExcluirMaintance();
        }

        //public void ExcluirMaintance2()
        //{
        //    var reservasAssociadas = new XPCollection<ReservaChurrasqueiraData>(Session,
        //        CriteriaOperator.Parse("GerenciarChurrasqueira.Oid = ? AND IsManutencao = true", Oid));

        //    var reservasParaExcluir = new List<ReservaChurrasqueiraData>();

        //    foreach (var reserva in reservasAssociadas)
        //    {
        //        reservasParaExcluir.Add(reserva);
        //    }

        //    foreach (var reserva in reservasParaExcluir)
        //    {
        //        reserva.Delete();
        //    }
           
        //}

        public void ExcluirMaintance()
        {
            var reservasAssociadas = new XPCollection<ReservaChurrasqueiraData>(Session,
                CriteriaOperator.Parse("GerenciarChurrasqueira.Oid = ? AND IsManutencao = true", Oid));

            var reservasParaExcluir = new List<ReservaChurrasqueiraData>();

            foreach (var reserva in reservasAssociadas)
            {
                reservasParaExcluir.Add(reserva);
            }

            foreach (var reserva in reservasParaExcluir)
            {
                reserva.Delete();
            }
        }

        public override void AfterConstruction()
        {
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