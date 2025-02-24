﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.Persistent.Base;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PopupNotesController : ViewController
    {
        private SimpleAction deleteAction;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public PopupNotesController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(ReservaChurrasqueiraData);
            TargetViewType = ViewType.ListView;
            PopupWindowShowAction showNotesAction = new PopupWindowShowAction(this, "MostrarNotasdeAção", PredefinedCategory.Edit)
            {
                ImageName = "Action_New",
                Caption = "Adicionar Bloqueios"
            };
            showNotesAction.CustomizePopupWindowParams += ShowNotesAction_CustomizePopupWindowParams;


        }

        private void ShowNotesAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateListView(typeof(GerenciarChurrasqueira), true);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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
    }
}
