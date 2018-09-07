﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.ViewModel
{
    internal class StyleViewModel : EditorViewModelBase
    {
        public StyleViewModel()
        {
            this.SelectedStyleEditor = new SelectedStyleViewModel(this);
        }

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Subtitle))
            {
                this.Styles = this.Document.Subtitle?.StyleSet;
                this.SelectedStyle = null;
            }
        }

        public void Delete(Style style)
        {
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            var index = this.styles.IndexOf(style);
            if (index == -1)
                throw new ArgumentException("The value is not in the set.");
            this.Document.Do(new DocumentAction("Delete style.", sub => sub.StyleSet.Remove(style), sub => sub.StyleSet.Insert(index, style)));
        }

        public void Add(Style style)
        {
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.Document.Do(new DocumentAction("Add style.", sub => sub.StyleSet.Add(style), sub => sub.StyleSet.Remove(style)));
        }

        public StyleSet Styles
        {
            get => this.styles;
            private set => Set(ref this.styles, value);
        }

        private StyleSet styles;

        public Style SelectedStyle
        {
            get => this.selected;
            set => Set(ref this.selected, value);
        }

        private Style selected;

        public class SelectedStyleViewModel : ViewModelBase
        {
            private static readonly Style defaultStyle = new Style("*Default");

            private StyleViewModel parent;
            private Style selectedStyle;
            private int selectedIndex;

            public override void Cleanup()
            {
                if (this.selectedStyle != null)
                    this.selectedStyle.PropertyChanged -= SelectedStyle_PropertyChanged;
                this.selectedStyle = null;
                this.parent.PropertyChanged -= this.Parent_PropertyChanged;
                this.parent = null;
                base.Cleanup();
            }

            public SelectedStyleViewModel(StyleViewModel parent)
            {
                this.parent = parent;
                this.selectedStyle = parent.SelectedStyle;
                parent.PropertyChanged += this.Parent_PropertyChanged;
                if (this.selectedStyle != null)
                {
                    this.selectedStyle.PropertyChanged += SelectedStyle_PropertyChanged;
                    this.selectedIndex = parent.Styles.IndexOf(this.selectedStyle);
                }
                this.Delete = new RelayCommand(() => this.parent.Delete(this.selectedStyle), () => this.selectedStyle != null);
                this.Rename = new RelayCommand(this.rename, () => this.newName != null);
            }

            private void rename()
            {
                var index = this.selectedIndex;
                var oldValue = this.selectedStyle;
                var newValue = oldValue.Clone(this.newName);
                this.parent.Document.Do(new DocumentAction("Change Name", sub =>
                {
                    var temp = this.parent.SelectedStyle == oldValue;
                    sub.StyleSet[index] = newValue;
                    if (temp)
                        this.parent.SelectedStyle = newValue;
                }, sub =>
                {
                    var temp = this.parent.SelectedStyle == newValue;
                    sub.StyleSet[index] = oldValue;
                    if (temp)
                        this.parent.SelectedStyle = oldValue;
                }));
            }

            private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(SelectedStyle))
                {
                    if (this.selectedStyle != null)
                        this.selectedStyle.PropertyChanged -= SelectedStyle_PropertyChanged;
                    this.selectedStyle = this.parent.SelectedStyle;
                    if (this.selectedStyle != null)
                    {
                        this.selectedStyle.PropertyChanged += SelectedStyle_PropertyChanged;
                        this.selectedIndex = this.parent.Styles.IndexOf(this.selectedStyle);
                        this.RaisePropertyChanged("");
                    }
                    this.newName = null;
                    this.Delete.RaiseCanExecuteChanged();
                    this.Rename.RaiseCanExecuteChanged();
                }
            }

            private void SelectedStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
            }

            public RelayCommand Delete
            {
                get;
                private set;
            }

            public RelayCommand Rename
            {
                get;
                private set;
            }

            public string Name
            {
                get => (this.selectedStyle ?? defaultStyle).Name;
                set
                {
                    if (this.selectedStyle != null && !string.IsNullOrWhiteSpace(value) && !this.parent.Styles.ContainsName(value) && this.selectedStyle.Name != value)
                    {
                        this.newName = value;
                    }
                    else
                    {
                        this.newName = null;
                    }
                    this.RaisePropertyChanged(nameof(this.Name));
                    this.Rename.RaiseCanExecuteChanged();
                }
            }

            private string newName;

            public AlignmentStyle Alignment
            {
                get => (this.selectedStyle ?? defaultStyle).Alignment;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Alignment;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Alignment", sub => sub.StyleSet[index].Alignment = value, sub => sub.StyleSet[index].Alignment = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Alignment));
                }
            }

            public bool Bold
            {
                get => (this.selectedStyle ?? defaultStyle).Bold;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Bold;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Bold", sub => sub.StyleSet[index].Bold = value, sub => sub.StyleSet[index].Bold = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Bold));
                }
            }

            public BorderStyle BorderStyle
            {
                get => (this.selectedStyle ?? defaultStyle).BorderStyle;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.BorderStyle;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change BorderStyle", sub => sub.StyleSet[index].BorderStyle = value, sub => sub.StyleSet[index].BorderStyle = oldValue)))
                        this.RaisePropertyChanged(nameof(this.BorderStyle));
                }
            }

            //public int Encoding
            //{
            //    get
            //    {
            //        if(selectedStyle == null)
            //            return default(int);
            //        return selectedStyle.Encoding;
            //    }
            //    set
            //    {
            //        if(selectedStyle == null)
            //            return;
            //        var index = selectedIndex;
            //        var oldValue = selectedStyle.Encoding;
            //        if(!parent.Document.TryDo(new DocumentAction("Change Encoding", sub => sub.StyleSet[index].Encoding = value, sub => sub.StyleSet[index].Encoding = oldValue)))
            //            RaisePropertyChanged(nameof(Encoding));
            //    }
            //}

            public string FontName
            {
                get => (this.selectedStyle ?? defaultStyle).FontName;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.FontName;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change FontName", sub => sub.StyleSet[index].FontName = value, sub => sub.StyleSet[index].FontName = oldValue)))
                        this.RaisePropertyChanged(nameof(this.FontName));
                }
            }

            public double FontSize
            {
                get => (this.selectedStyle ?? defaultStyle).FontSize;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.FontSize;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change FontSize", sub => sub.StyleSet[index].FontSize = value, sub => sub.StyleSet[index].FontSize = oldValue)))
                        this.RaisePropertyChanged(nameof(this.FontSize));
                }
            }

            public bool Italic
            {
                get => (this.selectedStyle ?? defaultStyle).Italic;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Italic;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Italic", sub => sub.StyleSet[index].Italic = value, sub => sub.StyleSet[index].Italic = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Italic));
                }
            }

            public int MarginL
            {
                get => (this.selectedStyle ?? defaultStyle).MarginL;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.MarginL;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginL", sub => sub.StyleSet[index].MarginL = value, sub => sub.StyleSet[index].MarginL = oldValue)))
                        this.RaisePropertyChanged(nameof(this.MarginL));
                }
            }

            public int MarginR
            {
                get => (this.selectedStyle ?? defaultStyle).MarginR;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.MarginR;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginR", sub => sub.StyleSet[index].MarginR = value, sub => sub.StyleSet[index].MarginR = oldValue)))
                        this.RaisePropertyChanged(nameof(this.MarginR));
                }
            }

            public int MarginV
            {
                get => (this.selectedStyle ?? defaultStyle).MarginV;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.MarginV;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginV", sub => sub.StyleSet[index].MarginV = value, sub => sub.StyleSet[index].MarginV = oldValue)))
                        this.RaisePropertyChanged(nameof(this.MarginV));
                }
            }

            public double Outline
            {
                get => (this.selectedStyle ?? defaultStyle).Outline;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Outline;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Outline", sub => sub.StyleSet[index].Outline = value, sub => sub.StyleSet[index].Outline = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Outline));
                }
            }

            public Color OutlineColor
            {
                get => (this.selectedStyle ?? defaultStyle).OutlineColor;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.OutlineColor;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change OutlineColor", sub => sub.StyleSet[index].OutlineColor = value, sub => sub.StyleSet[index].OutlineColor = oldValue)))
                        this.RaisePropertyChanged(nameof(this.OutlineColor));
                }
            }

            public Color PrimaryColor
            {
                get => (this.selectedStyle ?? defaultStyle).PrimaryColor;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.PrimaryColor;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change PrimaryColor", sub => sub.StyleSet[index].PrimaryColor = value, sub => sub.StyleSet[index].PrimaryColor = oldValue)))
                        this.RaisePropertyChanged(nameof(this.PrimaryColor));
                }
            }

            public double Rotation
            {
                get => (this.selectedStyle ?? defaultStyle).Rotation;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Rotation;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Rotation", sub => sub.StyleSet[index].Rotation = value, sub => sub.StyleSet[index].Rotation = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Rotation));
                }
            }

            public double ScaleX
            {
                get => (this.selectedStyle ?? defaultStyle).ScaleX;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.ScaleX;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ScaleX", sub => sub.StyleSet[index].ScaleX = value, sub => sub.StyleSet[index].ScaleX = oldValue)))
                        this.RaisePropertyChanged(nameof(this.ScaleX));
                }
            }

            public double ScaleY
            {
                get => (this.selectedStyle ?? defaultStyle).ScaleY;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.ScaleY;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ScaleY", sub => sub.StyleSet[index].ScaleY = value, sub => sub.StyleSet[index].ScaleY = oldValue)))
                        this.RaisePropertyChanged(nameof(this.ScaleY));
                }
            }

            public Color SecondaryColor
            {
                get => (this.selectedStyle ?? defaultStyle).SecondaryColor;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.SecondaryColor;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change SecondaryColor", sub => sub.StyleSet[index].SecondaryColor = value, sub => sub.StyleSet[index].SecondaryColor = oldValue)))
                        this.RaisePropertyChanged(nameof(this.SecondaryColor));
                }
            }

            public double Shadow
            {
                get => (this.selectedStyle ?? defaultStyle).Shadow;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Shadow;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Shadow", sub => sub.StyleSet[index].Shadow = value, sub => sub.StyleSet[index].Shadow = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Shadow));
                }
            }

            public Color ShadowColor
            {
                get => (this.selectedStyle ?? defaultStyle).ShadowColor;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.ShadowColor;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ShadowColor", sub => sub.StyleSet[index].ShadowColor = value, sub => sub.StyleSet[index].ShadowColor = oldValue)))
                        this.RaisePropertyChanged(nameof(this.ShadowColor));
                }
            }

            public double Spacing
            {
                get => (this.selectedStyle ?? defaultStyle).Spacing;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Spacing;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Spacing", sub => sub.StyleSet[index].Spacing = value, sub => sub.StyleSet[index].Spacing = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Spacing));
                }
            }

            public bool Strikeout
            {
                get => (this.selectedStyle ?? defaultStyle).Strikeout;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Strikeout;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Strikeout", sub => sub.StyleSet[index].Strikeout = value, sub => sub.StyleSet[index].Strikeout = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Strikeout));
                }
            }

            public bool Underline
            {
                get => (this.selectedStyle ?? defaultStyle).Underline;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Underline;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Underline", sub => sub.StyleSet[index].Underline = value, sub => sub.StyleSet[index].Underline = oldValue)))
                        this.RaisePropertyChanged(nameof(this.Underline));
                }
            }
        }

        public SelectedStyleViewModel SelectedStyleEditor
        {
            get;
            private set;
        }

        public override void Cleanup()
        {
            if (this.SelectedStyleEditor != null)
                this.SelectedStyleEditor.Cleanup();
            this.SelectedStyleEditor = null;
            this.styles = null;
            this.selected = null;
            base.Cleanup();
        }
    }
}
