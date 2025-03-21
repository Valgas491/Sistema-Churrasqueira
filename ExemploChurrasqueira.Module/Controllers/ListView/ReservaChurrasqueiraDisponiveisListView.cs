using System.Net.Http;
using System.Net.Mail;
using System.ServiceModel.Security;
using System.Text.Json;
using DevExpress.Data.Filtering;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Cms;
using DevExpress.Xpo;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
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
                newAction.Caption = "Gerar Reserva";
            }
            Filtros();           
            var delete = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            delete.Active.SetItemValue("Desabilitar", false);
            if(ObjectSpace != null)
            {
                MaintanceDelete();
            }
        }
        private async void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ApplicationUser UsuarioLogado = this.ObjectSpace.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
            var selectObjects = e.SelectedObjects.Cast<ReservaChurrasqueiraData>().ToList();
            var objectSpace = View.ObjectSpace;
            if (selectObjects.Any())
            {
                foreach(var item in selectObjects)
                {
                    var result = await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
                    {
                        title = "Digite sua senha!",
                        input = "text",
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
                    if (result.TryGetProperty("isConfirmed", out JsonElement isConfirmed) && isConfirmed.GetBoolean())
                    {
                        if (result.TryGetProperty("value", out JsonElement value) && !string.IsNullOrEmpty(value.GetString()))
                        {
                            string mensagem = value.GetString();
                            if (UsuarioLogado.ComparePassword($"{mensagem}"))
                            {
                                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                {
                                    title = "Exclusão Confirmada!",
                                    icon = "success",
                                    confirmButtonText = "OK"
                                });
                                objectSpace.Delete(selectObjects);
                                objectSpace.CommitChanges();

                            }
                            else
                            {
                                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                                {
                                    title = "Senha Incorreta!",
                                    icon = "error",
                                    confirmButtonText = "OK"
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
                                confirmButtonText = "OK"
                            });
                        }
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
            }
            else
            {
                await jsRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Nenhuma Reserva Selecionada!",
                    icon = "error",
                    confirmButtonText = "OK"
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
                ObjectSpace.CommitChanges();
            }
            
        }


    }
}
