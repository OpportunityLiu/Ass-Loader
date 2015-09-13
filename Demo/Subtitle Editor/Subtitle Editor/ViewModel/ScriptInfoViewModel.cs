﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SubtitleEditor.Model;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AssLoader;

namespace SubtitleEditor.ViewModel
{
    class ScriptInfoViewModel : EditorViewModelBase
    {
        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(Document.Subtitle))
            {
                if(info != null)
                    info.PropertyChanged -= Info_PropertyChanged;
                info = Document.Subtitle?.ScriptInfo;
                if(info != null)
                    info.PropertyChanged += Info_PropertyChanged;
                RaisePropertyChanged(null);
            }
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private ScriptInfo info;

        public string Title
        {
            get
            {
                return info.Title;
            }
            set
            {
                var oldValue = info.Title;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoTitle,
                    sub => sub.ScriptInfo.Title = value,
                    sub => sub.ScriptInfo.Title = oldValue));
            }
        }

        public string OriginalEditing
        {
            get
            {
                return info.OriginalEditing;
            }
            set
            {
                var oldValue = info.OriginalEditing;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalEditing,
                    sub => sub.ScriptInfo.OriginalEditing = value,
                    sub => sub.ScriptInfo.OriginalEditing = oldValue));
            }
        }

        public string OriginalScript
        {
            get
            {
                return info.OriginalScript;
            }
            set
            {
                var oldValue = info.OriginalScript;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalScript,
                    sub => sub.ScriptInfo.OriginalScript = value,
                    sub => sub.ScriptInfo.OriginalScript = oldValue));
            }
        }

        public string OriginalTiming
        {
            get
            {
                return info.OriginalTiming;
            }
            set
            {
                var oldValue = info.OriginalTiming;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalTiming,
                    sub => sub.ScriptInfo.OriginalTiming = value,
                    sub => sub.ScriptInfo.OriginalTiming = oldValue));
            }
        }

        public string OriginalTranslation
        {
            get
            {
                return info.OriginalTranslation;
            }
            set
            {
                var oldValue = info.OriginalTranslation;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoOriginalTranslation,
                    sub => sub.ScriptInfo.OriginalTranslation = value,
                    sub => sub.ScriptInfo.OriginalTranslation = oldValue));
            }
        }

        public string ScriptUpdatedBy
        {
            get
            {
                return info.ScriptUpdatedBy;
            }
            set
            {
                var oldValue = info.ScriptUpdatedBy;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoScriptUpdatedBy,
                    sub => sub.ScriptInfo.ScriptUpdatedBy = value,
                    sub => sub.ScriptInfo.ScriptUpdatedBy = oldValue));
            }
        }

        public string UpdateDetails
        {
            get
            {
                return info.UpdateDetails;
            }
            set
            {
                var oldValue = info.UpdateDetails;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoUpdateDetails,
                    sub => sub.ScriptInfo.UpdateDetails = value,
                    sub => sub.ScriptInfo.UpdateDetails = oldValue));
            }
        }

        public string SynchPoint
        {
            get
            {
                return info.SynchPoint;
            }
            set
            {
                var oldValue = info.SynchPoint;
                value = stringFormat(value);
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoSynchPoint,
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
            get
            {
                return info.Collisions;
            }
            set
            {
                var oldValue = info.Collisions;
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoCollisions,
                    sub => sub.ScriptInfo.Collisions = value,
                    sub => sub.ScriptInfo.Collisions = oldValue));
            }
        }

        public WrapStyle WrapStyle
        {
            get
            {
                return info.WrapStyle;
            }
            set
            {
                var oldValue = info.WrapStyle;
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoWrapStyle,
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
            get
            {
                return info.PlayResX;
            }
            set
            {
                var oldValue = info.PlayResX;
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoPlayResX,
                    sub => sub.ScriptInfo.PlayResX = value,
                    sub => sub.ScriptInfo.PlayResX = oldValue));
            }
        }

        public int PlayResY
        {
            get
            {
                return info.PlayResY;
            }
            set
            {
                var oldValue = info.PlayResY;
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoPlayResY,
                    sub => sub.ScriptInfo.PlayResY = value,
                    sub => sub.ScriptInfo.PlayResY = oldValue));
            }
        }

        public bool ScaledBorderAndShadow
        {
            get
            {
                return info.ScaledBorderAndShadow;
            }
            set
            {
                var oldValue = info.ScaledBorderAndShadow;
                if(value == oldValue)
                    return;
                Document.Do(new ScriptInfoAction(LocalizedStrings.Resources.ScriptInfoScaledBorderAndShadow,
                    sub => sub.ScriptInfo.ScaledBorderAndShadow = value,
                    sub => sub.ScriptInfo.ScaledBorderAndShadow = oldValue));
            }
        }

        private static string stringFormat(string value)
        {
            value = value.Trim();
            if(string.IsNullOrEmpty(value))
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
            if(info != null)
                info.PropertyChanged -= Info_PropertyChanged;
            info = null;
            base.Cleanup();
        }
    }
}
