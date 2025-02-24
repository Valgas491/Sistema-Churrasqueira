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
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Module.Controllers.Nested {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GerenciarNested : ViewController {
        private SimpleAction deleteAction;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public GerenciarNested() { 
            InitializeComponent();
            TargetObjectType = typeof(GerenciarChurrasqueira);
            TargetViewType = ViewType.ListView;

            deleteAction = new SimpleAction(this, "ExcluirBloqueio", PredefinedCategory.Edit)
            {
                Caption = "Excluir Bloqueio", 
                ImageName = "Action_Delete",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
            deleteAction.Execute += DeleteAction_Execute;
        }
        private void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.ObjectSpace;
            var selectedObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (selectedObjects.Any())
            {
                foreach (var item in selectedObjects)
                {
                    objectSpace.Delete(item);
                }
                objectSpace.CommitChanges();
                View.Refresh();
            }
        }
        protected override void OnActivated() {
            base.OnActivated(); 
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated(); 
            // Access and customize the target View control.
        }
        protected override void OnDeactivated() {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
