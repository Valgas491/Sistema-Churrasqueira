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
using ExemploChurrasqueira.Module.BusinessObjects.Logs;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.DetailView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ChurrasqueiraDetailView : ObjectViewController<DevExpress.ExpressApp.DetailView, Churrasqueira>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        private IJSRuntime jsRuntime;
        public ChurrasqueiraDetailView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
            base.OnActivated();
            // Perform various tasks depending on the target View.
            base.OnActivated();
            var saveAction = Frame.GetController<ModificationsController>()?.SaveAction;
            if (saveAction != null)
            {
                saveAction.Caption = "Salvar Churrasqueira";
                saveAction.Execute += SaveAction_Execute;
            }
        }

        private void SaveAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var select = e.SelectedObjects.Cast<Churrasqueira>().ToList();
            if (select.Any())
            {
                foreach (var item in select)
                {
                    var log = ObjectSpace.CreateObject<LogReservaChurrasqueiraData>();
                    log.DataHora = DateTime.Now;
                    log.Usuario = SecuritySystem.CurrentUserName;
                    log.Acao = "Criado";
                    log.Detalhes = $"Churrasqueira: {item.Nome} Criada, Data: {DateTime.Today:dd/MM/yyyy}";
                    log.Churrasqueira1 = item.Nome;
                    log.Local = "Criar Churrasqueira";
                    ObjectSpace.CommitChanges();
                }
                jsRuntime.InvokeVoidAsync("open", "/Churrasqueira_ListView", "_self");
            }
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
            var saveAction = Frame.GetController<ModificationsController>()?.SaveAction;
            saveAction.Execute -= SaveAction_Execute;
        }
    }
}
