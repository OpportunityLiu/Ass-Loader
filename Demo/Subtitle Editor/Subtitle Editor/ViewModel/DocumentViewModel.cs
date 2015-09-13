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
            doc.PropertyChanged += document_PropertyChanged;
            Redo = new RelayCommand(() => doc.Redo(), () => doc.RedoAction != null);
            Undo = new RelayCommand(() => doc.Undo(), () => doc.UndoAction != null);
        }

        private Document doc = ViewModelLocator.GetForCurrentView().Document;

        private void document_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName))
            {
                Redo.RaiseCanExecuteChanged();
                Undo.RaiseCanExecuteChanged();
                return;
            }
            switch(e.PropertyName)
            {
            case nameof(Document.UndoAction):
                RaisePropertyChanged(nameof(UndoHint));
                Undo.RaiseCanExecuteChanged();
                break;
            case nameof(Document.RedoAction):
                Redo.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(RedoHint));
                break;
            default:
                break;
            }
        }

        public string UndoHint => string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizedStrings.Resources.DocumentUndo, doc.UndoAction?.ActionFriendlyName);

        public string RedoHint => string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizedStrings.Resources.DocumentRedo, doc.RedoAction?.ActionFriendlyName);

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
            if(doc != null)
                doc.PropertyChanged += document_PropertyChanged;
            doc = null;
            base.Cleanup();
        }
    }
}
