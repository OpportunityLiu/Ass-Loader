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
            Redo = new RelayCommand(() => doc.Redo(), () => doc.CanRedo);
            Undo = new RelayCommand(() => doc.Undo(), () => doc.CanUndo);
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
            case nameof(Document.CanRedo):
                Redo.RaiseCanExecuteChanged();
                break;
            case nameof(Document.CanUndo):
                Undo.RaiseCanExecuteChanged();
                break;
            default:
                break;
            }
        }

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
    }
}
