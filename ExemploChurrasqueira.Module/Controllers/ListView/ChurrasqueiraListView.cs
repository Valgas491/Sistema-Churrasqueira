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
using ExemploChurrasqueira.Module.BusinessObjects.Logs;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ChurrasqueiraListView : ObjectViewController<DevExpress.ExpressApp.ListView, Churrasqueira>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public ChurrasqueiraListView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
                newAction.Caption = "Cadastro Churrasqueira";
            var exportAction = Frame.GetController<ExportController>()?.ExportAction;
            exportAction?.Active.SetItemValue("ListView", false);
            var deleteAction = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            if(deleteAction != null)
                deleteAction.Execute += DeleteAction_Execute;
        }

        private void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var select = e.SelectedObjects.Cast<Churrasqueira>().ToList();
            if (select.Any())
            {
                foreach(var item in select)
                {
                    var log = ObjectSpace.CreateObject<LogReservaChurrasqueiraData>();
                    log.DataHora = DateTime.Now;
                    log.Usuario = SecuritySystem.CurrentUserName;
                    log.Acao = "Excluído";
                    log.Detalhes = $"Exclusão de Churrasqueira, Data: {DateTime.Today:dd/MM/yyyy}";
                    log.Churrasqueira1 = item.Nome;
                    log.Local = "Criar Churrasqueira";
                    ObjectSpace.CommitChanges();
                }
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
            var exportAction = Frame.GetController<ExportController>()?.ExportAction;
            exportAction?.Active.SetItemValue("ListView", true);
            var deleteAction = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            deleteAction.Execute -= DeleteAction_Execute;
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
                newAction.Caption = "Novo";
        }
    }
}
