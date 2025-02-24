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

namespace ExemploChurrasqueira.Module.Controllers.DetailView {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class AgendaControllerDetailView : ObjectViewController<DevExpress.ExpressApp.DetailView, ReservaSchedulerModel> {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public AgendaControllerDetailView() { 
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private void ConfigurarBotoes(bool desabilitar)
        {
            Frame.GetController<ExportController>()?.ExportAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RefreshController>()?.RefreshAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<ModificationsController>()?.SaveAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<ModificationsController>()?.SaveAndNewAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RecordsNavigationController>()?.PreviousObjectAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RecordsNavigationController>()?.NextObjectAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<ModificationsController>()?.SaveAndCloseAction.Active.SetItemValue("DetailView", desabilitar);
        }

        protected override void OnActivated() {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            ConfigurarBotoes(false);
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated(); 
            // Access and customize the target View control.
        }
        protected override void OnDeactivated() {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            ConfigurarBotoes(true);
        }
    }
}
