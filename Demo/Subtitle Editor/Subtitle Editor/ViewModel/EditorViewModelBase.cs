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
            Document = ioc.Document;
            Document.PropertyChanged += Document_PropertyChanged;
            DocumentView = ioc.DocumentView;
            Document_PropertyChanged(Document, new PropertyChangedEventArgs(null));
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
            if(Document != null)
                Document.PropertyChanged -= Document_PropertyChanged;
            Document = null;
            DocumentView = null;
            base.Cleanup();
        }
    }
}
