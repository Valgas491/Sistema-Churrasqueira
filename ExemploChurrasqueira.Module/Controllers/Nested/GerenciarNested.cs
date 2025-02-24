using System.Text.Json;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.Nested {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GerenciarNested : ViewController {
        private SimpleAction deleteAction;
        private SimpleAction statusAction;
        private IJSRuntime jsRuntime;
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

            statusAction = new SimpleAction(this, "AlterarStatus", PredefinedCategory.Edit)
            {
                Caption = "Alterar Status Para Concluido",
                ImageName = "Action_MarkCompleted",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
            statusAction.Execute += StatusAction_Execute;
        }

        private async void StatusAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.ObjectSpace;
            var selectObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (selectObjects.Any())
            {
                foreach (var item in selectObjects)
                {
                    if (item.Status == GerenciarChurrasqueira.TaskStatus.Maintance && item.DataManutencao > DateTime.Today)
                    {
                        var result = await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
                        {
                            title = "Confirmação",
                            text = "Deseja realmente marcar como concluído?",
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
                                title = "Status alterado para concluído antes do prazo! A Churrasqueira será liberada para fazer Reserva",
                                icon = "success",
                                confirmButtonText = "OK"
                            });
                            objectSpace.CommitChanges();
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
                        objectSpace.CommitChanges();
                    }
                }
            }
        }


        private async void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
            }
        }


        protected override void OnActivated() {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
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
