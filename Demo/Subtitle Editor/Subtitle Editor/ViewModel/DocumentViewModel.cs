using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Commands;
using Opportunity.MvvmUniverse.Views;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.ViewModel
{
    internal class DocumentViewModel : ViewModelBase
    {
        public static ResourceInfo.Resources.ViewModel.IDocument Localizer { get; } = LocalizedStrings.Resources.ViewModel.Document;

        public DocumentViewModel()
        {
            this.doc.PropertyChanged += this.document_PropertyChanged;
        }

        private readonly Document doc = ThreadLocalSingleton.GetOrCreate<Document>();

        private void document_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.Redo.OnCanExecuteChanged();
                this.Undo.OnCanExecuteChanged();
                this.OnObjectReset();
                return;
            }
            switch (e.PropertyName)
            {
            case nameof(Document.UndoAction):
                this.OnPropertyChanged(nameof(this.UndoHint));
                this.Undo.OnCanExecuteChanged();
                break;
            case nameof(Document.RedoAction):
                this.Redo.OnCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.RedoHint));
                break;
            default:
                break;
            }
        }

        public string UndoHint => Localizer.Undo(this.doc.UndoAction?.ActionFriendlyName);

        public string RedoHint => Localizer.Redo(this.doc.RedoAction?.ActionFriendlyName);

        public Command Redo => Commands.GetOrAdd(() => Command.Create(c => this.doc.Redo(), c => this.doc.RedoAction != null));

        public Command Undo => Commands.GetOrAdd(() => Command.Create(c => this.doc.Undo(), c => this.doc.UndoAction != null));
    }
}
