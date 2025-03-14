﻿using System.Text;
using System.Text.Json;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.XtraExport;
using DevExpress.XtraPrinting;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PopupNotesController : ViewController
    {
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
    public class ActionInPopupController : ViewController
    {
        private IJSRuntime jsRuntime;
        public ActionInPopupController()
        {
            TargetViewType = ViewType.ListView;
            SimpleAction deleteAction = new SimpleAction(
                this, "Excluir", DevExpress.Persistent.Base.PredefinedCategory.PopupActions)
            {
                Caption = "Excluir Bloqueio",
                ImageName = "Actions_Delete"
            };
            //Refer to the https://docs.devexpress.com/eXpressAppFramework/112815 help article to see how to reorder Actions within the PopupActions container.
            deleteAction.Execute += DeleteAction_Execute;

            SimpleAction statusAction = new SimpleAction(
                this, "Concluido", DevExpress.Persistent.Base.PredefinedCategory.PopupActions)
            {
                Caption = "Finalizado",
                ImageName = "Action_MarkCompleted"
            };
            //Refer to the https://docs.devexpress.com/eXpressAppFramework/112815 help article to see how to reorder Actions within the PopupActions container.
            statusAction.Execute += StatusAction_Execute;

            SimpleAction exportAction = new SimpleAction(
                this, "Exportar", DevExpress.Persistent.Base.PredefinedCategory.PopupActions)
            {
                Caption = "Exportar",
                ImageName = "Action_Export"
            };
            //Refer to the https://docs.devexpress.com/eXpressAppFramework/112815 help article to see how to reorder Actions within the PopupActions container.
            exportAction.Execute += ExportAction_Execute;
            Actions.Add(exportAction);
        }

        private async void ExportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selectedObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (!selectedObjects.Any())
            {
                return;
            }
            var exportData = selectedObjects.Select(c => new
            {
                c.QtdDias,
                c.DataManutencao,
                c.Churrasqueira.Nome
            }).ToList();
            var csvData = new StringBuilder();
            csvData.AppendLine("Nome, QtdDias, DataManutencao");
            foreach (var item in exportData)
            {
                csvData.AppendLine($"{item.Nome}, {item.QtdDias}, {item.DataManutencao:yyyy-MM-dd}");
            }
            string fileName = $"Churrasqueiras_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var jsRuntime = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IJSRuntime>();
            await jsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(Encoding.UTF8.GetBytes(csvData.ToString())));
        }



        private async void StatusAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.ObjectSpace;
            var selectObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (selectObjects.Any())
            {
                foreach (var item in selectObjects)
                {
                    if (item.Status == GerenciarChurrasqueira.TaskStatus.Maintance && item.DataManutencao.AddDays(item.QtdDias) > DateTime.Today)
                    {
                        var result = await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
                        {
                            title = "Confirmação",
                            text = "Deseja realmente marcar como concluído, antes do prazo?",
                            icon = "warning",
                            showCancelButton = true,
                            confirmButtonText = "Sim, concluir!",
                            cancelButtonText = "Cancelar"
                        });

                        if (result.TryGetProperty("isConfirmed", out JsonElement isConfirmed) && isConfirmed.GetBoolean())
                        {
                            item.Status = GerenciarChurrasqueira.TaskStatus.Completed;
                            await jsRuntime.InvokeVoidAsync("Swal.fire", new
                            {
                                title = "Status alterado para concluído antes do prazo!",
                                text = "A Churrasqueira será liberada para fazer Reserva",
                                icon = "success",
                                confirmButtonText = "OK"
                            });
                            objectSpace.CommitChanges();
                            await jsRuntime.InvokeVoidAsync("open", "ReservaChurrasqueiraData_ListView", "_self");
                        }
                        else
                        {
                            await jsRuntime.InvokeVoidAsync("Swal.fire", new
                            {
                                title = "Ação Cancelada!",
                                icon = "error",
                                confirmButtonText = "OK"
                            });
                        }
                    }
                    else
                    {
                        item.Status = GerenciarChurrasqueira.TaskStatus.Completed;
                        await jsRuntime.InvokeVoidAsync("Swal.fire", new
                        {
                            title = "Status alterado para concluído!",
                            icon = "success",
                            confirmButtonText = "OK"
                        });
                        objectSpace.CommitChanges();
                        await jsRuntime.InvokeVoidAsync("open", "/ReservaChurrasqueiraData_ListView", "_self");
                    }
                }
            }
        }
        async void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.ObjectSpace;
            var selectedObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (selectedObjects.Any())
            {
                foreach (var item in selectedObjects)
                {
                    objectSpace.Delete(item);
                }
                await Task.Delay(500);
                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Manutenção excluída.",
                    icon = "success",
                    confirmButtonText = "OK"
                });
                objectSpace.CommitChanges();
                View.Refresh();
                await jsRuntime.InvokeVoidAsync("open", "/ReservaChurrasqueiraData_ListView", "_self");
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
        }

    }

  
}
