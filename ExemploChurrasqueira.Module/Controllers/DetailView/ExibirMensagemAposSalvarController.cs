using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.DetailView {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ExibirMensagemAposSalvarController : ObjectViewController<DevExpress.ExpressApp.DetailView, ReservaChurrasqueiraData>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        private IJSRuntime jsRuntime;
        public ExibirMensagemAposSalvarController() { 
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated() {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;

            if (ObjectSpace != null)
            {
                ObjectSpace.Committed += ObjectSpace_Committed;
            }
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated(); 
            // Access and customize the target View control.
        }
        protected override void OnDeactivated() {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            if (ObjectSpace != null)
            {
                ObjectSpace.Committed -= ObjectSpace_Committed;
            }
        }
        private async void ObjectSpace_Committed(object sender, System.EventArgs e)
        {
            if (jsRuntime != null && View.CurrentObject is ReservaChurrasqueiraData reservaLocal)
            {
                await Task.Delay(600);
                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Reserva salva com sucesso!",
                    icon = "success",
                    confirmButtonText = "OK"
                });
                await jsRuntime.InvokeVoidAsync("open", $"ReservaChurrasqueiraData_ListView", "_self");
            }
        }

    }
}
