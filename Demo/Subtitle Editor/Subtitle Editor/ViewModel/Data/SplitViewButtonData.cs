using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.ViewModel
{
    class SplitViewButtonData : ObservableObject
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

        private ICommand command;

        public ICommand Command
        {
            get
            {
                return command;
            }
            set
            {
                Set(ref command, value);
            }
        }
    }
}
