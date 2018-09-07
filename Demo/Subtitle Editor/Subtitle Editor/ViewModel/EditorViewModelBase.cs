using GalaSoft.MvvmLight;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.ViewModel
{
    abstract class EditorViewModelBase : ViewModelBase
    {
        public EditorViewModelBase()
        {
            var ioc = ViewModelLocator.GetForCurrentView();
            this.Document = ioc.Document;
            this.Document.PropertyChanged += this.Document_PropertyChanged;
            this.DocumentView = ioc.DocumentView;
            this.Document_PropertyChanged(this.Document, new PropertyChangedEventArgs(null));
        }

        protected virtual void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected Document Document
        {
            get;
            private set;
        }

        public DocumentViewModel DocumentView
        {
            get;
            private set;
        }

        public override void Cleanup()
        {
            if(this.Document != null)
                this.Document.PropertyChanged -= this.Document_PropertyChanged;
            this.Document = null;
            this.DocumentView = null;
            base.Cleanup();
        }
    }
}
