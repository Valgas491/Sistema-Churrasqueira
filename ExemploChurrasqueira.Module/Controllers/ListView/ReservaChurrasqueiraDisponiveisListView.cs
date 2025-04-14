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
    public partial class ReservaChurrasqueiraDisponiveisListView : ObjectViewController<DevExpress.ExpressApp.ListView, ReservaChurrasqueiraData>
    {
        #region UserConfig
        private ApplicationUser GetCurrentUser()
        {
            return this.ObjectSpace.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
        }
        #endregion

        #region Propriedades
        private string defaultMessage;
        private DeleteObjectsViewController deleteObjectsViewController;
        private IJSRuntime jsRuntime;
        private CustomLogonParameters logonParameters;
        #endregion

        #region Inicialização e Modificadores de Acesso 
        public ReservaChurrasqueiraDisponiveisListView()
        {
            InitializeComponent();
            SetupDeleteAction();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            InitializeServices();
            ConfigureUI();
            Filtros();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ResetUIConfiguration();
        }
        #endregion

        #region Métodos
        private void SetupDeleteAction()
        {
            SimpleAction exportAction = new SimpleAction(
                this, "Deletar Reserva", DevExpress.Persistent.Base.PredefinedCategory.Edit)
            {
                Caption = "Deletar Reserva",
                ImageName = "Action_Delete"
            };
            exportAction.Execute += DeleteAction_Execute;
        }
        private void InitializeServices()
        {
            jsRuntime = Application.ServiceProvider.GetService(typeof(IJSRuntime)) as IJSRuntime;
        }
        private void ConfigureUI()
        {
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
            {
                newAction.Caption = "Criar Reserva";
            }

            var delete = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            delete.Active.SetItemValue("Desabilitar", false);
        }
        private async void DeleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selectedObjects = e.SelectedObjects.Cast<ReservaChurrasqueiraData>().ToList();
            if (!selectedObjects.Any())
            {
                await ShowAlert("Nenhuma Reserva Selecionada!", "error");
                return;
            }

            var (isConfirmed, reason) = await GetDeletionReason();
            if (!isConfirmed || string.IsNullOrEmpty(reason))
            {
                await ShowCancellationAlert();
                return;
            }

            var (passwordConfirmed, password) = await GetUserPassword();
            if (!passwordConfirmed || string.IsNullOrEmpty(password))
            {
                await ShowCancellationAlert();
                return;
            }

            await ProcessReservationDeletions(selectedObjects, reason, password);
        }
        private bool ValidateDeletionReason(string reason)
        {
            return reason.Length > 3;
        }
        private bool ValidateUserPassword(ApplicationUser user, string password)
        {
            return user.ComparePassword(password);
        }
        private void CreateDeletionLog(ReservaChurrasqueiraData item, string reason)
        {
            var log = ObjectSpace.CreateObject<LogReservaChurrasqueiraData>();
            log.DataHora = DateTime.Now;
            log.Usuario = SecuritySystem.CurrentUserName;
            log.Acao = "Excluído";
            log.Detalhes = $"Reserva: {item.Associado}, Data da Reserva Excluída: {item.DataReserva_Churrasqueira:dd/MM/yyyy}, Motivo: {reason}";
            log.Churrasqueira1 = item.Churrasqueira.Nome;
            log.Local = "Reservar Churrasqueira";
        }
        private void DeleteReservation(ReservaChurrasqueiraData item)
        {
            ObjectSpace.Delete(item);
            ObjectSpace.CommitChanges();
        }
        private void HandleError(Exception ex)
        {
            Console.WriteLine($"Erro ao exibir alerta: {ex.Message}");
        }
        private void Filtros()
        {
            ((DevExpress.ExpressApp.ListView)View).CollectionSource.Criteria["DataFilter"] =
                CriteriaOperator.Parse("DataReserva_Churrasqueira >= ?", DateTime.Today);
            ((DevExpress.ExpressApp.ListView)View).CollectionSource.Criteria["QtdFilter"] =
                CriteriaOperator.Parse("QtdPessoas >= ?", 1);
        }
        private void ResetUIConfiguration()
        {
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            if (newAction != null)
            {
                newAction.Caption = "Criar";
            }

            var delete = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
            delete.Active.SetItemValue("Desabilitar", true);
        }
        #endregion

        #region Task
        private async Task<(bool IsConfirmed, string Reason)> GetDeletionReason()
        {
            var result = await ShowReasonPrompt();
            bool isConfirmed = result.TryGetProperty("isConfirmed", out JsonElement confirmed) && confirmed.GetBoolean();
            string reason = result.TryGetProperty("value", out JsonElement value) ? value.GetString() : string.Empty;

            return (isConfirmed, reason);
        }

        private async Task<JsonElement> ShowReasonPrompt()
        {
            return await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Digite o motivo da exclusão!",
                input = "text",
                inputAttributes = new { autocapitalize = "on" },
                showCancelButton = true,
                confirmButtonText = "Confirmar",
                cancelButtonText = "Cancelar",
                showLoaderOnConfirm = true,
                allowOutsideClick = false
            });
        }

        private async Task<(bool IsConfirmed, string Password)> GetUserPassword()
        {
            var result = await ShowPasswordPrompt();
            bool isConfirmed = result.TryGetProperty("isConfirmed", out JsonElement confirmed) && confirmed.GetBoolean();
            string password = result.TryGetProperty("value", out JsonElement value) ? value.GetString() : string.Empty;

            return (isConfirmed, password);
        }

        private async Task<JsonElement> ShowPasswordPrompt()
        {
            return await jsRuntime.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "Digite sua senha!",
                input = "password",
                inputAttributes = new { autocapitalize = "off" },
                showCancelButton = true,
                confirmButtonText = "Confirmar",
                cancelButtonText = "Cancelar",
                showLoaderOnConfirm = true,
                allowOutsideClick = false
            });
        }

        private async Task ProcessReservationDeletions(List<ReservaChurrasqueiraData> items, string reason, string password)
        {
            ApplicationUser UsuarioLogado = GetCurrentUser();

            foreach (var item in items)
            {
                if (!ValidateDeletionReason(reason))
                {
                    await ShowAlert("Quantidade de caracteres em motivo inválida! o minímo permitido é 4 caracteres", "error");
                    continue;
                }

                if (!ValidateUserPassword(UsuarioLogado, password))
                {
                    await ShowAlert("Senha Incorreta!", "error");
                    continue;
                }

                await ProcessValidDeletion(item, reason);
            }
        }

        private async Task ProcessValidDeletion(ReservaChurrasqueiraData item, string reason)
        {
            try
            {
                int i = 1;
                CreateDeletionLog(item, reason);
                await ShowSuccessAlert();
                DeleteReservation(item);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private async Task ShowSuccessAlert()
        {
            await ShowAlert("Exclusão Confirmada!", "success", 4000);
        }

        private async Task ShowCancellationAlert()
        {
            await ShowAlert("Ação Cancelada!", "error", 4000);
        }

        private async Task ShowAlert(string title, string icon, int? timer = null)
        {
            try
            {
                var alertOptions = new
                {
                    title,
                    icon,
                    confirmButtonText = "OK",
                    timer
                };

                await jsRuntime.InvokeVoidAsync("Swal.fire", alertOptions);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }
        #endregion
    
    }
}