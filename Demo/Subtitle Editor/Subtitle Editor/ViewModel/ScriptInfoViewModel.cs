using GalaSoft.MvvmLight;
using Opportunity.AssLoader;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.ViewModel
{
    internal class ScriptInfoViewModel : EditorViewModelBase
    {
        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Subtitle))
            {
                if (this.info != null)
                    this.info.PropertyChanged -= this.Info_PropertyChanged;
                this.info = this.Document.Subtitle?.ScriptInfo;
                if (this.info != null)
                    this.info.PropertyChanged += this.Info_PropertyChanged;
                this.RaisePropertyChanged(null);
            }
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoTitle,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalEditing,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalScript,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalTiming,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalTranslation,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoScriptUpdatedBy,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoUpdateDetails,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoSynchPoint,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoCollisions,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoWrapStyle,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoPlayResX,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoPlayResY,
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
                this.Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoScaledBorderAndShadow,
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

        private class ScriptInfoAction : DocumentAction
        {
            public ScriptInfoAction(string propertyName, DocumentActionDo doAction, DocumentActionUndo undoAction)
                : base(string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizedStrings.Resources.ActionSetScriptInfo, propertyName), doAction, undoAction)
            {
            }
        }

        public override void Cleanup()
        {
            if (this.info != null)
                this.info.PropertyChanged -= this.Info_PropertyChanged;
            this.info = null;
            base.Cleanup();
        }
    }
}
