using System.Text.Json;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using ExemploChurrasqueira.Module.BusinessObjects.Logs;
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
            SimpleAction deleteAction3 = new SimpleAction(
               this, "Deletar Reserva3", DevExpress.Persistent.Base.PredefinedCategory.Edit)
            {
                Caption = "Deletar Manutenção",
                ImageName = "Action_Delete"
            };
            deleteAction3.Execute += DeleteAction_Execute;

            SimpleAction alterarAction = new SimpleAction(
               this, "Alterar Status", DevExpress.Persistent.Base.PredefinedCategory.Edit)
            {
                Caption = "Alterar Status",
                ImageName = "Action_Validation_Validate"
            };
            alterarAction.Execute += StatusAction_Execute;

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
            var deleteorigin = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            deleteorigin.Active.SetItemValue("Desablitar", false);
            if (ObjectSpace != null)
            {
                MaintanceDelete();
                DeletarDuplicataManutencao();
            }


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
                denyButtonText = "Não salvar",
                timer = 4000
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
        private async void StatusAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.ObjectSpace;
            var selectObjects = e.SelectedObjects.Cast<GerenciarChurrasqueira>().ToList();

            if (selectObjects.Any())
            {
                foreach (var item in selectObjects)
                {
                    if(item.Status == GerenciarChurrasqueira.TaskStatus.Maintance)
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
                                cancelButtonText = "Cancelar",
                                timer = 4000
                            });

                            if (result.TryGetProperty("isConfirmed", out JsonElement isConfirmed) && isConfirmed.GetBoolean())
                            {
                                item.Status = GerenciarChurrasqueira.TaskStatus.Completed;
                                objectSpace.CommitChanges();
                                try
                                {
                                    await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                    {
                                        title = "Status alterado para concluído antes do prazo!",
                                        text = "A Churrasqueira será liberada para fazer Reserva",
                                        icon = "success",
                                        confirmButtonText = "OK",
                                        timer = 4000
                                    });
                                }
                                catch (Exception ex)
                                {
                                    // Aqui você pode logar o erro ou exibir uma mensagem alternativa, se desejar
                                    Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
                                }
                                await jsRuntime.InvokeVoidAsync("open", "/GerenciarChurrasqueira_ListView", "_self");
                                
                            }
                            else
                            {
                                try
                                {
                                    await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                    {
                                        title = "Ação Cancelada!",
                                        icon = "error",
                                        confirmButtonText = "OK",
                                        timer = 4000
                                    });
                                }
                                catch (Exception ex)
                                {
                                    // Aqui você pode logar o erro ou exibir uma mensagem alternativa, se desejar
                                    Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            item.Status = GerenciarChurrasqueira.TaskStatus.Completed;
                            objectSpace.CommitChanges();
                            try
                            {
                                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                {
                                    title = "Status alterado para concluído!",
                                    icon = "success",
                                    confirmButtonText = "OK",
                                    timer = 4000
                                });
                            }
                            catch (Exception ex)
                            {
                                // Aqui você pode logar o erro ou exibir uma mensagem alternativa, se desejar
                                Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
                            }
                            await jsRuntime.InvokeVoidAsync("open", "/GerenciarChurrasqueira_ListView", "_self");
                        }
                    }
                    else
                    {
                        try
                        {
                            await jsRuntime.InvokeVoidAsync("Swal.fire", new
                            {
                                title = "Status já está como Finalizado.",
                                icon = "error",
                                confirmButtonText = "OK",
                                timer = 4000
                            });
                        }
                        catch (Exception ex)
                        {
                            // Aqui você pode logar o erro ou exibir uma mensagem alternativa, se desejar
                            Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
                        }
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
                    var log = ObjectSpace.CreateObject<LogReservaChurrasqueiraData>();
                    log.DataHora = DateTime.Now;
                    log.Usuario = SecuritySystem.CurrentUserName;
                    log.Acao = "Excluído";
                    log.Detalhes = $"Manutenção de Churrasqueira Excluída, para a Churrasqueira {item.Churrasqueira.Nome}, Data: {DateTime.Today:dd/MM/yyyy}";
                    log.Churrasqueira1 = item.Churrasqueira.Nome;
                    log.Local = "Gerenciar Manutenção";
                    ObjectSpace.CommitChanges();
                }
                foreach (var item in selectedObjects)
                {
                    objectSpace.Delete(item);
                    objectSpace.CommitChanges();
                }
                await Task.Delay(500);
                try
                {
                    await jsRuntime.InvokeVoidAsync("Swal.fire", new
                    {
                        title = "Manutenção excluída.",
                        icon = "success",
                        confirmButtonText = "OK",
                        timer = 4000
                    });
                }
                catch (Exception ex)
                {
                    // Aqui você pode logar o erro ou exibir uma mensagem alternativa, se desejar
                    Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
                }
            }
        }
        private void MaintanceDelete()
        {
            var reservasManutencaoConcluidas = ObjectSpace.GetObjects<ReservaChurrasqueiraData>()
                .Where(r => r.IsManutencao == true && r.GerenciarChurrasqueira.Status.Equals(GerenciarChurrasqueira.TaskStatus.Completed) && r.DataReserva_Churrasqueira > DateTime.Today)
                .ToList();

            foreach (var reserva in reservasManutencaoConcluidas)
            {
                ObjectSpace.Delete(reserva);
                ObjectSpace.CommitChanges();
            }

        }
        private void DeletarDuplicataManutencao()
        {

            var duplicados = ObjectSpace.GetObjects<ReservaChurrasqueiraData>()
                .GroupBy(r => new {
                    r.ClassInfo,
                    r.GerenciarChurrasqueira,
                    r.Churrasqueira,
                    r.DataReserva_Churrasqueira
                })
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var grupo in duplicados)
            {

                var registrosParaExcluir = grupo.Skip(1).ToList();
                foreach (var registro in registrosParaExcluir)
                {
                    ObjectSpace.Delete(registro);
                }
            }

            ObjectSpace.CommitChanges();
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
            var deleteorigin = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            deleteorigin.Active.SetItemValue("Desablitar", true);

        }
    }
}
