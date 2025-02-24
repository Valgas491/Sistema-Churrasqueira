using DevExpress.ExpressApp;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using ExemploChurrasqueira.Module.Helper;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GerenciarListVIew : ObjectViewController<DevExpress.ExpressApp.ListView, GerenciarChurrasqueira>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        private IJSRuntime jsRuntime;
        public GerenciarListVIew()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
            //if (ObjectSpace != null)
            //{
            //    ObjectSpace.Committed += ObjectSpace_Committed;
            //}

        }
        public async Task ObjectSpace_Committed()
        {
            await Task.Delay(600);

            var result = await jsRuntime.InvokeAsync<object>("Swal.fire", new
            {
                title = "Deseja salvar as alterações?",
                showDenyButton = true,
                showCancelButton = true,
                confirmButtonText = "Salvar",
                denyButtonText = "Não salvar"
            });

            if (result != null && result.ToString() == "confirmed")
            {
                ToastHelper.Toast("Alterações foram salvas.", InformationType.Warning);
            }
            else
            {
                await jsRuntime.InvokeVoidAsync("open", $"ReservaChurrasqueiraData_ListView", "_self");
            }
        }


        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            
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
            //if (ObjectSpace != null)
            //{
            //    ObjectSpace.Committed -= ObjectSpace_Committed;
            //}
        }
    }
}
