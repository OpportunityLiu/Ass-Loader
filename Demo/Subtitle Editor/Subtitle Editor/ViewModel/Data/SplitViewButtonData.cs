using Opportunity.MvvmUniverse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.ViewModel
{
    internal class SplitViewButtonData : ObservableObject
    {
        private string icon;

        public string Icon
        {
            get => this.icon;
            set => this.Set(ref this.icon, value);
        }

        private string content;

        public string Content
        {
            get => this.content;
            set => this.Set(ref this.content, value);
        }

        private ICommand command;

        public ICommand Command
        {
            get => this.command;
            set => this.Set(ref this.command, value);
        }
    }
}
