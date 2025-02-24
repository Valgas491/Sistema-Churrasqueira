using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraCharts.Native;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{

    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [DefaultClassOptions]
    [DomainComponent]
    [XafDisplayName("Agenda")]
    public class ReservaSchedulerModel : IXafEntityObject/*, IObjectSpaceLink*/, INotifyPropertyChanged
    {
        public ReservaSchedulerModel()
        {
            Oid = Guid.NewGuid();
        }
        //private IObjectSpace objectSpace;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        List<ReservaChurrasqueiraData> listaReservas;

        
        public List<ReservaChurrasqueiraData> ListaReservas
        {
            get => listaReservas;
            set
            {
                if(listaReservas != value)
                {
                    listaReservas = value;
                    OnPropertyChanged();
                }
            }
        }

        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]  // Hide the entity identifier from UI.
        public Guid Oid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void IXafEntityObject.OnCreated()
        {
            
        }

        void IXafEntityObject.OnLoaded()
        {
            
        }

        void IXafEntityObject.OnSaving()
        {
            
        }
    }
}