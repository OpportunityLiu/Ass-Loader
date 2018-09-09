using Opportunity.AssLoader;
using Opportunity.Helpers.ObjectModel;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.ViewModel
{
    internal class ScriptInfoViewModel : EditorViewModelBase
    {
        public static ResourceInfo.Resources.ViewModel.IScriptInfo Localizer { get; } = LocalizedStrings.Resources.ViewModel.ScriptInfo;

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Subtitle))
            {
                this.info = this.Document.Subtitle?.ScriptInfo;
                this.OnObjectReset();
            }
        }

        private ScriptInfo info;

        public string Title
        {
            get => this.info.Title;
            set
            {
                var oldValue = this.info.Title;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.Title = value,
                    sub => sub.ScriptInfo.Title = oldValue));
            }
        }

        public string OriginalEditing
        {
            get => this.info.OriginalEditing;
            set
            {
                var oldValue = this.info.OriginalEditing;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.OriginalEditing = value,
                    sub => sub.ScriptInfo.OriginalEditing = oldValue));
            }
        }

        public string OriginalScript
        {
            get => this.info.OriginalScript;
            set
            {
                var oldValue = this.info.OriginalScript;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.OriginalScript = value,
                    sub => sub.ScriptInfo.OriginalScript = oldValue));
            }
        }

        public string OriginalTiming
        {
            get => this.info.OriginalTiming;
            set
            {
                var oldValue = this.info.OriginalTiming;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.OriginalTiming = value,
                    sub => sub.ScriptInfo.OriginalTiming = oldValue));
            }
        }

        public string OriginalTranslation
        {
            get => this.info.OriginalTranslation;
            set
            {
                var oldValue = this.info.OriginalTranslation;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.OriginalTranslation = value,
                    sub => sub.ScriptInfo.OriginalTranslation = oldValue));
            }
        }

        public string ScriptUpdatedBy
        {
            get => this.info.ScriptUpdatedBy;
            set
            {
                var oldValue = this.info.ScriptUpdatedBy;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.ScriptUpdatedBy = value,
                    sub => sub.ScriptInfo.ScriptUpdatedBy = oldValue));
            }
        }

        public string UpdateDetails
        {
            get => this.info.UpdateDetails;
            set
            {
                var oldValue = this.info.UpdateDetails;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.UpdateDetails = value,
                    sub => sub.ScriptInfo.UpdateDetails = oldValue));
            }
        }

        public string SynchPoint
        {
            get => this.info.SynchPoint;
            set
            {
                var oldValue = this.info.SynchPoint;
                value = stringFormat(value);
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.SynchPoint = value,
                    sub => sub.ScriptInfo.SynchPoint = oldValue));
            }
        }

        //public double? Timer
        //{
        //    get
        //    {
        //        return info.Timer;
        //    }
        //    set
        //    {
        //        var oldValue = info.Timer;
        //        if(value == oldValue)
        //            return;
        //        Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoTimer,
        //            sub => sub.ScriptInfo.Timer = value,
        //            sub => sub.ScriptInfo.Timer = oldValue));
        //    }
        //}

        public CollisionStyle Collisions
        {
            get => this.info.Collisions;
            set
            {
                var oldValue = this.info.Collisions;
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.Collisions = value,
                    sub => sub.ScriptInfo.Collisions = oldValue));
            }
        }

        public WrapStyle WrapStyle
        {
            get => this.info.WrapStyle;
            set
            {
                var oldValue = this.info.WrapStyle;
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.WrapStyle = value,
                    sub => sub.ScriptInfo.WrapStyle = oldValue));
            }
        }

        //public int? PlayDepth
        //{
        //    get
        //    {
        //        return info.PlayDepth;
        //    }
        //    set
        //    {
        //        var oldValue = info.PlayDepth;
        //        if(value == oldValue)
        //            return;
        //        Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoPlayDepth,
        //            sub => sub.ScriptInfo.PlayDepth = value,
        //            sub => sub.ScriptInfo.PlayDepth = oldValue));
        //    }
        //}

        public int PlayResX
        {
            get => this.info.PlayResX;
            set
            {
                var oldValue = this.info.PlayResX;
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.PlayResX = value,
                    sub => sub.ScriptInfo.PlayResX = oldValue));
            }
        }

        public int PlayResY
        {
            get => this.info.PlayResY;
            set
            {
                var oldValue = this.info.PlayResY;
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.PlayResY = value,
                    sub => sub.ScriptInfo.PlayResY = oldValue));
            }
        }

        public bool ScaledBorderAndShadow
        {
            get => this.info.ScaledBorderAndShadow;
            set
            {
                var oldValue = this.info.ScaledBorderAndShadow;
                if (value == oldValue)
                    return;
                this.Document.Do(new ScriptInfoAction(
                    sub => sub.ScriptInfo.ScaledBorderAndShadow = value,
                    sub => sub.ScriptInfo.ScaledBorderAndShadow = oldValue));
            }
        }

        private static string stringFormat(string value)
        {
            value = value.Trim();
            if (string.IsNullOrEmpty(value))
                return null;
            return value;
        }

        private sealed class ScriptInfoAction : DocumentAction
        {
            public ScriptInfoAction(DocumentActionDo doAction, DocumentActionUndo undoAction, [CallerMemberName]string propertyName = null)
                : base(Localizer.SetScriptInfo(Localizer.GetValue(propertyName)), doAction, undoAction)
            {
                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new ArgumentNullException(nameof(propertyName));
                PropertyName = propertyName;
            }

            public string PropertyName { get; }

            public override void Do(Subtitle<ScriptInfo> subtitle)
            {
                base.Do(subtitle);
                ThreadLocalSingleton.Get<ScriptInfoViewModel>().OnPropertyChanged(PropertyName);
            }

            public override void Undo(Subtitle<ScriptInfo> subtitle)
            {
                base.Undo(subtitle);
                ThreadLocalSingleton.Get<ScriptInfoViewModel>().OnPropertyChanged(PropertyName);
            }
        }
    }
}
