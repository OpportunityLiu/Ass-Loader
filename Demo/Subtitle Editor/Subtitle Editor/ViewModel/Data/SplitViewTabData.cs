using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.ViewModel
{
    class SplitViewTabData : ObservableObject
    {
        private string icon;

        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                Set(ref icon, value);
            }
        }

        private string content;

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                Set(ref content, value);
            }
        }

        private Type pageType;

        public Type PageType
        {
            get
            {
                return pageType;
            }
            set
            {
                Set(ref pageType, value);
            }
        }

        private bool isChecked = false;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                Set(ref isChecked, value);
            }
        }
    }
}
