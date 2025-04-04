using System.Text.Json;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using ExemploChurrasqueira.Module.BusinessObjects.Logs;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.JSInterop;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ReservaChurrasqueiraDisponiveisListView : ObjectViewController<DevExpress.ExpressApp.ListView, ReservaChurrasqueiraData>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        private string defaultMessage;
        DeleteObjectsViewController deleteObjectsViewController;
        private IJSRuntime jsRuntime;
        private CustomLogonParameters logonParameters;
        public ReservaChurrasqueiraDisponiveisListView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            SimpleAction exportAction = new SimpleAction(
               this, "Deletar Reserva", DevExpress.Persistent.Base.PredefinedCategory.Edit)
            {
                Caption = "Deletar Reserva",
                ImageName = "Action_Delete"
            };
            //Refer to the https://docs.devexpress.com/eXpressAppFramework/112815 help article to see how to reorder Actions within the PopupActions container.
            exportAction.Execute += DeleteAction_Execute;
            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
            // Perform various tasks depending on the target View.
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
            {
                newAction.Caption = "Criar Reserva";
            }
            Filtros();           
            var delete = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            delete.Active.SetItemValue("Desabilitar", false);
            
        }
        private async void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ApplicationUser UsuarioLogado = this.ObjectSpace.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
            var selectObjects = e.SelectedObjects.Cast<ReservaChurrasqueiraData>().ToList();
            var objectSpace = View.ObjectSpace;
            if (selectObjects.Any())
            {
                var result2 = await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
                {
                    title = "Digite o motivo da exclusão!",
                    input = "text",
                    inputAttributes = new
                    {
                        autocapitalize = "on"
                    },
                    showCancelButton = true,
                    confirmButtonText = "Confirmar",
                    cancelButtonText = "Cancelar",
                    showLoaderOnConfirm = true,
                    allowOutsideClick = false
                });
                var result = await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
                {
                    title = "Digite sua senha!",
                    input = "password",
                    inputAttributes = new
                    {
                        autocapitalize = "off"
                    },
                    showCancelButton = true,
                    confirmButtonText = "Confirmar",
                    cancelButtonText = "Cancelar",
                    showLoaderOnConfirm = true,
                    allowOutsideClick = false
                });
                foreach (var item in selectObjects)
                {         
                    if ((result.TryGetProperty("isConfirmed", out JsonElement isConfirmed) && isConfirmed.GetBoolean()) && (result2.TryGetProperty("isConfirmed", out JsonElement isConfirmed2) && isConfirmed2.GetBoolean()))
                    {
                        if ((result.TryGetProperty("value", out JsonElement value) && !string.IsNullOrEmpty(value.GetString()))&&(result2.TryGetProperty("value", out JsonElement value2) && !string.IsNullOrEmpty(value2.GetString())))
                        {
                            string mensagem = value.GetString();
                            string mensagem2 = value2.GetString().ToLower();

                            if(mensagem2.Length > 3)
                            {
                                if (UsuarioLogado.ComparePassword($"{mensagem}"))
                                {
                                    var log = ObjectSpace.CreateObject<LogReservaChurrasqueiraData>();
                                    log.DataHora = DateTime.Now;
                                    log.Usuario = SecuritySystem.CurrentUserName;
                                    log.Acao = "Excluído";
                                    log.Detalhes = $"Reserva: {item.Associado}, Data da Reserva Excluída: {item.DataReserva_Churrasqueira:dd/MM/yyyy}, Motivo: {mensagem2}";
                                    log.Churrasqueira1 = item.Churrasqueira.Nome;
                                    log.Local = "Reservar Churrasqueira";
                                    await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                    {
                                        title = "Exclusão Confirmada!",
                                        icon = "success",
                                        confirmButtonText = "OK",
                                        timer = 4000
                                    });
                                    objectSpace.Delete(item);
                                    objectSpace.CommitChanges();

                                }
                                else
                                {
                                    await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                    {
                                        title = "Senha Incorreta!",
                                        icon = "error",
                                        confirmButtonText = "OK",
                                        timer = 4000
                                    });

                                }
                            }
                            else
                            {
                                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                {
                                    title = "Quantidade de caracteres em motivo invalida!",
                                    icon = "error",
                                    confirmButtonText = "OK",
                                    timer = 4000
                                });
                            }
                            
                        }
                        else
                        {
                            await jsRuntime.InvokeVoidAsync("Swal.fire", new
                            {
                                title = "Campo vazio!",
                                text = "Você precisa digitar uma mensagem.",
                                icon = "error",
                                confirmButtonText = "OK",
                                timer = 4000
                            });
                        }
                    }
                    else
                    {
                        await jsRuntime.InvokeVoidAsync("Swal.fire", new
                        {
                            title = "Ação Cancelada!",
                            icon = "error",
                            confirmButtonText = "OK",
                            timer = 4000
                        });
                    }
                
                }
            }
            else
            {
                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Nenhuma Reserva Selecionada!",
                    icon = "error",
                    confirmButtonText = "OK",
                    timer = 4000
                });
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
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
            {
                newAction.Caption = "Criar";
            }
            var delete = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            delete.Active.SetItemValue("Desabilitar", true);

        }

        private void Filtros()
        {
            // Desativa a visualização das reservas passadas
            ((DevExpress.ExpressApp.ListView)View).CollectionSource.Criteria["DataFilter"] =
                CriteriaOperator.Parse("DataReserva_Churrasqueira >= ?", DateTime.Today);
            ((DevExpress.ExpressApp.ListView)View).CollectionSource.Criteria["QtdFilter"] =
                CriteriaOperator.Parse("QtdPessoas >= ?", 1);
        }
        
        

    }
}
