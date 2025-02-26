using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.DetailView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GerenciarChurrsqueiraDetailView : ObjectViewController<DevExpress.ExpressApp.DetailView, GerenciarChurrasqueira>
    {
        private IJSRuntime jsRuntime;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public GerenciarChurrsqueiraDetailView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
            base.OnActivated();
            AoAlterarData();
            var saveACtion = Frame.GetController<ModificationsController>()?.SaveAction;
            saveACtion.Execute += SaveACtion_Execute;
            var saveACtionNew = Frame.GetController<ModificationsController>()?.SaveAndNewAction;
            saveACtionNew?.Active.SetItemValue("DetailView", false);
        }

        private async void SaveACtion_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            await jsRuntime.InvokeVoidAsync("open", "/ReservaChurrasqueiraData_ListView", "_self");
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        private void AoAlterarData()
        {

            if (View is DevExpress.ExpressApp.DetailView detailView)
            {
                var date = detailView.FindItem("DataManutencao") as PropertyEditor;
                if (date == null)
                {
                    MostrarToast("Campo de data não encontrado.", InformationType.Error);
                    return;
                }

                date.ControlValueChanged += (sender, evento) =>
                {
                    // Atualiza o valor no objeto
                    date.WriteValue();

                    // Obtém o valor atualizado da data
                    var novaData = date.PropertyValue as DateTime?;
                    if (!novaData.HasValue)
                    {
                        MostrarToast("Data inválida!", InformationType.Error);
                        return;
                    }

                    // Obtém o objeto atual
                    var gerenciarChurrasqueira = View.CurrentObject as GerenciarChurrasqueira;
                    var objectSpace = View.ObjectSpace;

                    // Verifica churrasqueiras disponíveis
                    var churrasqueirasDisponiveis = ObterChurrasqueirasDisponiveis(objectSpace, novaData.Value);

                    if (churrasqueirasDisponiveis.Any())
                    {
                        // Atualiza a lista transitória
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.Clear();
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.AddRange(churrasqueirasDisponiveis);
                        MostrarToast($"{churrasqueirasDisponiveis.Count} churrasqueira(s) disponível(is).", InformationType.Success);
                    }
                    else
                    {
                        MostrarToast("Nenhuma churrasqueira disponível na data selecionada.", InformationType.Warning);
                    }

                    // Atualiza a interface
                    View.Refresh();
                };
            }
        }

        private List<Churrasqueira> ObterChurrasqueirasDisponiveis(IObjectSpace objectSpace, DateTime dataReserva_Churrasqueira)
        {
            // Busca churrasqueiras já reservadas ou em manutenção para a data fornecida
            var churrasqueirasIndisponiveis = objectSpace.GetObjects<ReservaChurrasqueiraData>(
                CriteriaOperator.Parse("(DataReserva_Churrasqueira = ? AND IsManutencao = false) OR (DataReserva_Churrasqueira = ? AND IsManutencao = true)",
                dataReserva_Churrasqueira, dataReserva_Churrasqueira))
                .Select(r => r.Churrasqueira).Distinct().ToList();

            // Busca todas as churrasqueiras
            var todasChurrasqueiras = objectSpace.GetObjects<Churrasqueira>();

            // Filtra as disponíveis
            var churrasqueirasDisponiveis = todasChurrasqueiras
                .Where(c => !churrasqueirasIndisponiveis.Contains(c))
                .ToList();

            return churrasqueirasDisponiveis;
        }

        private void MostrarToast(string mensagem, DevExpress.ExpressApp.InformationType tipo)
        {

            var showViewStrategy = Application.ShowViewStrategy as ShowViewStrategyBase;
            showViewStrategy?.ShowMessage(mensagem, tipo); // Tipos: "success", "error", "warning", "info"
        }

    }
}
