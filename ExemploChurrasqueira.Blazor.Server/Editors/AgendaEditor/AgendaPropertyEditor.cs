using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Microsoft.AspNetCore.Components;

namespace ExemploChurrasqueira.Blazor.Server.Editors.AgendaEditor
{
    [PropertyEditor(typeof(List<ReservaChurrasqueiraData>), false)]
    public class AgendaPropertyEditor : BlazorPropertyEditorBase
    {
        public AgendaPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override IComponentAdapter CreateComponentAdapter() => new AgendaInputAdapter(new AgendaInputModel());
    }
}
