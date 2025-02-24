using DevExpress.ExpressApp.Blazor.Components.Models;
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Blazor.Server.Editors.AgendaEditor
{
    public class AgendaInputModel : ComponentModelBase
    {
        public List<ReservaChurrasqueiraData> Value
        {
            get => GetPropertyValue<List<ReservaChurrasqueiraData>>();
            set => SetPropertyValue(value);
        }
        public bool ReadOnly
        {
            get => GetPropertyValue<bool>();
            set => SetPropertyValue(value);
        }

        public void SetValueFromUI(List<ReservaChurrasqueiraData> value)
        {
            SetPropertyValue(value, notify: false, nameof(Value));
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ValueChanged;
    }
}
