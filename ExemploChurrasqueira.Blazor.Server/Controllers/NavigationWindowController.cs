using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Blazor.Server.Controllers {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class NavigationWindowController : WindowController {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public NavigationWindowController() {
            InitializeComponent();
            TargetWindowType = WindowType.Main;
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated() {
            base.OnActivated();
            base.OnActivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem +=
                new EventHandler<CustomShowNavigationItemEventArgs>(ViewController1_CustomShowNavigationItem);
            // Perform various tasks depending on the target Window.
        }
        protected override void OnDeactivated() {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        void ViewController1_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            ChoiceActionItem selectedItem = e.ActionArguments.SelectedChoiceActionItem;

            //Botão de Agenda
            if (selectedItem != null && selectedItem.Id == "AgendaNavigation")
            {
                NonPersistentObjectSpace os = (NonPersistentObjectSpace)Application.CreateObjectSpace(typeof(ReservaSchedulerModel));
                os.PopulateAdditionalObjectSpaces(Application);
                os.AutoRefreshAdditionalObjectSpaces = true;
                os.AutoCommitAdditionalObjectSpaces = true;
                os.AutoDisposeAdditionalObjectSpaces = true;

                ReservaSchedulerModel agendaPage = os.CreateObject<ReservaSchedulerModel>();

                e.ActionArguments.ShowViewParameters.CreatedView = Application.CreateDetailView(os, agendaPage, true);
                e.ActionArguments.ShowViewParameters.TargetWindow = TargetWindow.Current;
                e.Handled = true;
            }
        }
    }
}
