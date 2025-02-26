using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using ExemploChurrasqueira.Module.Helper;



namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class IngressoReportController : ObjectViewController<DevExpress.ExpressApp.ListView, ReservaChurrasqueiraData>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public IngressoReportController()
        {
            InitializeComponent();
            SimpleAction generateTicketAction = new SimpleAction(
                this, "GerarIngresso", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Baixar Ingresso",
                ImageName = "Action_Printing_Preview"
            };
            generateTicketAction.Execute += BtnGerarVisualizacaoIngressos_Execute;
        }

        private void BtnGerarVisualizacaoIngressos_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var eventos = View.SelectedObjects.Cast<ReservaChurrasqueiraData>().ToList();

            if (eventos == null || !eventos.Any())
            {
                MessageOptions msg = ToastHelper.Toast("Nenhum evento selecionado.", InformationType.Warning);
                Application.ShowViewStrategy.ShowMessage(msg);
                return;
            }

            foreach (var evento in eventos)
            {
                IngressoReport report = new IngressoReport();
                report.DataSource = new List<ReservaChurrasqueiraData> { evento }; 
                report.CreateDocument();

                string caminhoArquivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"IngressoDeChurrasqueira_{evento.Oid}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                report.ExportToPdf(caminhoArquivo);
                MessageOptions msgDownload = ToastHelper.Toast($"Ingresso gerado: Na Pasta Documentos", InformationType.Success);
                Application.ShowViewStrategy.ShowMessage(msgDownload);
                Process.Start(new ProcessStartInfo(caminhoArquivo) { UseShellExecute = true });
            }

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