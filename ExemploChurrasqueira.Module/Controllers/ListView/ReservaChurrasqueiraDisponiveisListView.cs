﻿using DevExpress.Data.Filtering;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Utils;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ReservaChurrasqueiraDisponiveisListView : ObjectViewController<DevExpress.ExpressApp.ListView, ReservaChurrasqueiraData>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public ReservaChurrasqueiraDisponiveisListView()
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
            {
                newAction.Caption = "Gerar Reserva";
            }
            
            Filtros();
            MaintanceDelete();
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
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
            {
                newAction.Caption = "Criar";
            }
        }
        private void Filtros()
        {
            
            // Desativa a visualização das reservas passadas
            ((DevExpress.ExpressApp.ListView)View).CollectionSource.Criteria["DataFilter"] =
                CriteriaOperator.Parse("DataReserva_Churrasqueira >= ?", DateTime.Today);
           

        }
        
        private void MaintanceDelete()
        {
            var reservasManutencaoConcluidas = ObjectSpace.GetObjects<ReservaChurrasqueiraData>()
                .Where(r => r.IsManutencao == true && r.GerenciarChurrasqueira.Status.Equals(GerenciarChurrasqueira.TaskStatus.Completed)&& r.DataReserva_Churrasqueira > DateTime.Today)
                .ToList();

            foreach (var reserva in reservasManutencaoConcluidas)
            {
                ObjectSpace.Delete(reserva);
            }
            ObjectSpace.CommitChanges();
        }
    }
}
