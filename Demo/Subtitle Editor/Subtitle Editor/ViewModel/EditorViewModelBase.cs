using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Views;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.ViewModel
{
    internal abstract class EditorViewModelBase : ViewModelBase
    {
        public EditorViewModelBase()
        {
            this.Document.PropertyChanged += this.Document_PropertyChanged;
            this.Document_PropertyChanged(this.Document, new PropertyChangedEventArgs(null));
        }

        protected virtual void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected Document Document { get; } = ThreadLocalSingleton.GetOrCreate<Document>();

        public DocumentViewModel DocumentView { get; } = ThreadLocalSingleton.GetOrCreate<DocumentViewModel>();
    }
}
