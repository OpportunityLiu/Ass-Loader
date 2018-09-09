using Opportunity.MvvmUniverse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.ViewModel
{
    internal class SplitViewTabData : ObservableObject
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

        private Type pageType;
        public Type PageType
        {
            get => this.pageType;
            set => this.Set(ref this.pageType, value);
        }

        private bool isChecked = false;
        public bool IsChecked
        {
            get => this.isChecked;
            set => this.Set(ref this.isChecked, value);
        }
    }
}
