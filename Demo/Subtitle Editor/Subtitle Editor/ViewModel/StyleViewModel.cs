using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using AssLoader;
using System.ComponentModel;
using AssLoader.Collections;

namespace SubtitleEditor.ViewModel
{
    class StyleViewModel : EditorViewModelBase
    {
        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(Document.Subtitle))
            {
                Styles = Document.Subtitle?.StyleSet;
                SelectedStyle = null;
            }
        }

        public StyleSet Styles
        {
            get
            {
                return styles;
            }
            private set
            {
                Set(ref styles, value);
            }
        }

        private StyleSet styles;

        public Style SelectedStyle
        {
            get
            {
                return selected;
            }
            set
            {
                Set(ref selected, value);
            }
        }

        private Style selected;
    }
}
