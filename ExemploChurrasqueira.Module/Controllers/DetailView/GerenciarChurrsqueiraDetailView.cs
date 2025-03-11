using System.Text.Json;
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
            if (saveACtion != null)
            {
                saveACtion.Execute += async (s, e) => await SaveACtion_Execute(s, e);
            }
            
            var saveACtionNew = Frame.GetController<ModificationsController>()?.SaveAndNewAction;
            saveACtionNew?.Active.SetItemValue("DetailView", false);
        }

        private async Task SaveACtion_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            await Task.Delay(100);
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
                    date.WriteValue();
                    var novaData = date.PropertyValue as DateTime?;
                    if (!novaData.HasValue)
                    {
                        MostrarToast("Data inválida!", InformationType.Error);
                        return;
                    }

                    var gerenciarChurrasqueira = View.CurrentObject as GerenciarChurrasqueira;
                    var objectSpace = View.ObjectSpace;

                    var churrasqueirasDisponiveis = ObterChurrasqueirasDisponiveis(objectSpace, novaData.Value);

                    if (churrasqueirasDisponiveis.Any())
                    {
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.Clear();
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.AddRange(churrasqueirasDisponiveis);
                        MostrarToast($"{churrasqueirasDisponiveis.Count} churrasqueira(s) disponível(is).", InformationType.Success);
                    }
                    else
                    {
                        MostrarToast("Nenhuma churrasqueira disponível na data selecionada.", InformationType.Warning);
                    }

                    View.Refresh();
                };
            }
        }

        private List<Churrasqueira> ObterChurrasqueirasDisponiveis(IObjectSpace objectSpace, DateTime dataReserva_Churrasqueira)
        {

            var churrasqueirasIndisponiveis = objectSpace.GetObjects<ReservaChurrasqueiraData>(
                CriteriaOperator.Parse("(DataReserva_Churrasqueira = ? AND IsManutencao = false) OR (DataReserva_Churrasqueira = ? AND IsManutencao = true)",
                dataReserva_Churrasqueira, dataReserva_Churrasqueira))
                .Select(r => r.Churrasqueira).Distinct().ToList();

            var todasChurrasqueiras = objectSpace.GetObjects<Churrasqueira>();

            var churrasqueirasDisponiveis = todasChurrasqueiras
                .Where(c => !churrasqueirasIndisponiveis.Contains(c))
                .ToList();

            return churrasqueirasDisponiveis;
        }

        private void MostrarToast(string mensagem, DevExpress.ExpressApp.InformationType tipo)
        {

            var showViewStrategy = Application.ShowViewStrategy as ShowViewStrategyBase;
            showViewStrategy?.ShowMessage(mensagem, tipo);
        }

        public async void ExisteReserva(DateTime dataReservaAtual)
        {
            await jsRuntime.InvokeVoidAsync("Swal.fire", new
            {
                title = $"Na Data: {dataReservaAtual},existe uma reserva de associado.",
                icon = "success",
                confirmButtonText = "OK"
            });
        }

    }
}
