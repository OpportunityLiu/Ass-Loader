using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using SubtitleEditor.Model;

namespace SubtitleEditor.ViewModel
{
    class DocumentViewModel : ViewModelBase
    {
        public DocumentViewModel()
        {
            this.doc.PropertyChanged += this.document_PropertyChanged;
            this.Redo = new RelayCommand(() => this.doc.Redo(), () => this.doc.RedoAction != null);
            this.Undo = new RelayCommand(() => this.doc.Undo(), () => this.doc.UndoAction != null);
        }

        private Document doc = ViewModelLocator.GetForCurrentView().Document;

        private void document_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName))
            {
                this.Redo.RaiseCanExecuteChanged();
                this.Undo.RaiseCanExecuteChanged();
                return;
            }
            switch(e.PropertyName)
            {
            case nameof(Document.UndoAction):
                this.RaisePropertyChanged(nameof(this.UndoHint));
                this.Undo.RaiseCanExecuteChanged();
                break;
            case nameof(Document.RedoAction):
                this.Redo.RaiseCanExecuteChanged();
                this.RaisePropertyChanged(nameof(this.RedoHint));
                break;
            default:
                break;
            }
        }

        public string UndoHint => string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizedStrings.Resources.DocumentUndo, this.doc.UndoAction?.ActionFriendlyName);

        public string RedoHint => string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizedStrings.Resources.DocumentRedo, this.doc.RedoAction?.ActionFriendlyName);

        public RelayCommand Redo
        {
            get;
            private set;
        }

        public RelayCommand Undo
        {
            get;
            private set;
        }

        public override void Cleanup()
        {
            if(this.doc != null)
                this.doc.PropertyChanged += this.document_PropertyChanged;
            this.doc = null;
            base.Cleanup();
        }
    }
}
