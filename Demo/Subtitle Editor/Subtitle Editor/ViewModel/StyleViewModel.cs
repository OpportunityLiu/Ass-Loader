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
using SubtitleEditor.Model;

namespace SubtitleEditor.ViewModel
{
    class StyleViewModel : EditorViewModelBase
    {
        public StyleViewModel()
        {
            this.SelectedStyleEditor = new SelectedStyleViewModel(this);
        }

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(Document.Subtitle))
            {
                Styles = Document.Subtitle?.StyleSet;
                SelectedStyle = null;
            }
        }

        public void Delete(Style style)
        {
            if(style == null)
                throw new ArgumentNullException(nameof(style));
            var index = styles.IndexOf(style);
            if(index == -1)
                throw new ArgumentException("The value is not in the set.");
            Document.Do(new DocumentAction("Delete style.", sub => sub.StyleSet.Remove(style), sub => sub.StyleSet.Insert(index, style)));
        }

        public void Add(Style style)
        {
            if(style == null)
                throw new ArgumentNullException(nameof(style));
            Document.Do(new DocumentAction("Add style.", sub => sub.StyleSet.Add(style), sub => sub.StyleSet.Remove(style)));
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

        public class SelectedStyleViewModel : ViewModelBase
        {
            private StyleViewModel parent;
            private Style selectedStyle;
            private int selectedIndex;

            public override void Cleanup()
            {
                if(selectedStyle != null)
                    selectedStyle.PropertyChanged -= SelectedStyle_PropertyChanged;
                selectedStyle = null;
                parent.PropertyChanged -= Parent_PropertyChanged;
                parent = null;
                base.Cleanup();
            }

            public SelectedStyleViewModel(StyleViewModel parent)
            {
                this.parent = parent;
                this.selectedStyle = parent.SelectedStyle;
                parent.PropertyChanged += Parent_PropertyChanged;
                if(selectedStyle != null)
                {
                    selectedStyle.PropertyChanged += SelectedStyle_PropertyChanged;
                    this.selectedIndex = parent.Styles.IndexOf(selectedStyle);
                }
            }

            private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(SelectedStyle))
                {
                    if(selectedStyle != null)
                        selectedStyle.PropertyChanged -= SelectedStyle_PropertyChanged;
                    selectedStyle = parent.SelectedStyle;
                    if(selectedStyle != null)
                    {
                        selectedStyle.PropertyChanged += SelectedStyle_PropertyChanged;
                        selectedIndex = parent.Styles.IndexOf(selectedStyle);
                    }
                    RaisePropertyChanged(null);
                }
            }

            private void SelectedStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                RaisePropertyChanged(e.PropertyName);
            }

            public string Name
            {
                get
                {
                    if(selectedStyle == null)
                        return default(string);
                    return selectedStyle.Name;
                }
                set
                {
                    if(selectedStyle == null || string.IsNullOrWhiteSpace(value) || parent.Styles.ContainsName(value))
                    {
                        RaisePropertyChanged(nameof(Name));
                        return;
                    }
                    var index = selectedIndex;
                    var oldValue = selectedStyle;
                    var newValue = selectedStyle.Clone(value);
                    parent.Document.Do(new DocumentAction("Change Name", sub =>
                    {
                        var temp = parent.SelectedStyle == oldValue;
                        sub.StyleSet.RemoveAt(index);
                        sub.StyleSet.Insert(index, newValue);
                        if(temp)
                            parent.SelectedStyle = newValue;
                    }, sub =>
                    {
                        var temp = parent.SelectedStyle == newValue;
                        sub.StyleSet.RemoveAt(index);
                        sub.StyleSet.Insert(index, oldValue);
                        if(temp)
                            parent.SelectedStyle = oldValue;
                    }));
                }
            }

            public AlignmentStyle Alignment
            {
                get
                {
                    if(selectedStyle == null)
                        return default(AlignmentStyle);
                    return selectedStyle.Alignment;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Alignment;
                    if(!parent.Document.TryDo(new DocumentAction("Change Alignment", sub => sub.StyleSet[index].Alignment = value, sub => sub.StyleSet[index].Alignment = oldValue)))
                        RaisePropertyChanged(nameof(Alignment));
                }
            }

            public bool Bold
            {
                get
                {
                    if(selectedStyle == null)
                        return default(bool);
                    return selectedStyle.Bold;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Bold;
                    if(!parent.Document.TryDo(new DocumentAction("Change Bold", sub => sub.StyleSet[index].Bold = value, sub => sub.StyleSet[index].Bold = oldValue)))
                        RaisePropertyChanged(nameof(Bold));
                }
            }

            public BorderStyle BorderStyle
            {
                get
                {
                    if(selectedStyle == null)
                        return default(BorderStyle);
                    return selectedStyle.BorderStyle;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.BorderStyle;
                    if(!parent.Document.TryDo(new DocumentAction("Change BorderStyle", sub => sub.StyleSet[index].BorderStyle = value, sub => sub.StyleSet[index].BorderStyle = oldValue)))
                        RaisePropertyChanged(nameof(BorderStyle));
                }
            }

            public int Encoding
            {
                get
                {
                    if(selectedStyle == null)
                        return default(int);
                    return selectedStyle.Encoding;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Encoding;
                    if(!parent.Document.TryDo(new DocumentAction("Change Encoding", sub => sub.StyleSet[index].Encoding = value, sub => sub.StyleSet[index].Encoding = oldValue)))
                        RaisePropertyChanged(nameof(Encoding));
                }
            }

            public string FontName
            {
                get
                {
                    if(selectedStyle == null)
                        return default(string);
                    return selectedStyle.FontName;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.FontName;
                    if(!parent.Document.TryDo(new DocumentAction("Change FontName", sub => sub.StyleSet[index].FontName = value, sub => sub.StyleSet[index].FontName = oldValue)))
                        RaisePropertyChanged(nameof(FontName));
                }
            }

            public double FontSize
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.FontSize;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.FontSize;
                    if(!parent.Document.TryDo(new DocumentAction("Change FontSize", sub => sub.StyleSet[index].FontSize = value, sub => sub.StyleSet[index].FontSize = oldValue)))
                        RaisePropertyChanged(nameof(FontSize));
                }
            }

            public bool Italic
            {
                get
                {
                    if(selectedStyle == null)
                        return default(bool);
                    return selectedStyle.Italic;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Italic;
                    if(!parent.Document.TryDo(new DocumentAction("Change Italic", sub => sub.StyleSet[index].Italic = value, sub => sub.StyleSet[index].Italic = oldValue)))
                        RaisePropertyChanged(nameof(Italic));
                }
            }

            public int MarginL
            {
                get
                {
                    if(selectedStyle == null)
                        return default(int);
                    return selectedStyle.MarginL;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.MarginL;
                    if(!parent.Document.TryDo(new DocumentAction("Change MarginL", sub => sub.StyleSet[index].MarginL = value, sub => sub.StyleSet[index].MarginL = oldValue)))
                        RaisePropertyChanged(nameof(MarginL));
                }
            }

            public int MarginR
            {
                get
                {
                    if(selectedStyle == null)
                        return default(int);
                    return selectedStyle.MarginR;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.MarginR;
                    if(!parent.Document.TryDo(new DocumentAction("Change MarginR", sub => sub.StyleSet[index].MarginR = value, sub => sub.StyleSet[index].MarginR = oldValue)))
                        RaisePropertyChanged(nameof(MarginR));
                }
            }

            public int MarginV
            {
                get
                {
                    if(selectedStyle == null)
                        return default(int);
                    return selectedStyle.MarginV;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.MarginV;
                    if(!parent.Document.TryDo(new DocumentAction("Change MarginV", sub => sub.StyleSet[index].MarginV = value, sub => sub.StyleSet[index].MarginV = oldValue)))
                        RaisePropertyChanged(nameof(MarginV));
                }
            }

            public double Outline
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.Outline;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Outline;
                    if(!parent.Document.TryDo(new DocumentAction("Change Outline", sub => sub.StyleSet[index].Outline = value, sub => sub.StyleSet[index].Outline = oldValue)))
                        RaisePropertyChanged(nameof(Outline));
                }
            }

            public Color OutlineColor
            {
                get
                {
                    if(selectedStyle == null)
                        return default(Color);
                    return selectedStyle.OutlineColor;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.OutlineColor;
                    if(!parent.Document.TryDo(new DocumentAction("Change OutlineColor", sub => sub.StyleSet[index].OutlineColor = value, sub => sub.StyleSet[index].OutlineColor = oldValue)))
                        RaisePropertyChanged(nameof(OutlineColor));
                }
            }

            public Color PrimaryColor
            {
                get
                {
                    if(selectedStyle == null)
                        return default(Color);
                    return selectedStyle.PrimaryColor;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.PrimaryColor;
                    if(!parent.Document.TryDo(new DocumentAction("Change PrimaryColor", sub => sub.StyleSet[index].PrimaryColor = value, sub => sub.StyleSet[index].PrimaryColor = oldValue)))
                        RaisePropertyChanged(nameof(PrimaryColor));
                }
            }

            public double Rotation
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.Rotation;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Rotation;
                    if(!parent.Document.TryDo(new DocumentAction("Change Rotation", sub => sub.StyleSet[index].Rotation = value, sub => sub.StyleSet[index].Rotation = oldValue)))
                        RaisePropertyChanged(nameof(Rotation));
                }
            }

            public double ScaleX
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.ScaleX;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.ScaleX;
                    if(!parent.Document.TryDo(new DocumentAction("Change ScaleX", sub => sub.StyleSet[index].ScaleX = value, sub => sub.StyleSet[index].ScaleX = oldValue)))
                        RaisePropertyChanged(nameof(ScaleX));
                }
            }

            public double ScaleY
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.ScaleY;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.ScaleY;
                    if(!parent.Document.TryDo(new DocumentAction("Change ScaleY", sub => sub.StyleSet[index].ScaleY = value, sub => sub.StyleSet[index].ScaleY = oldValue)))
                        RaisePropertyChanged(nameof(ScaleY));
                }
            }

            public Color SecondaryColor
            {
                get
                {
                    if(selectedStyle == null)
                        return default(Color);
                    return selectedStyle.SecondaryColor;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.SecondaryColor;
                    if(!parent.Document.TryDo(new DocumentAction("Change SecondaryColor", sub => sub.StyleSet[index].SecondaryColor = value, sub => sub.StyleSet[index].SecondaryColor = oldValue)))
                        RaisePropertyChanged(nameof(SecondaryColor));
                }
            }

            public double Shadow
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.Shadow;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Shadow;
                    if(!parent.Document.TryDo(new DocumentAction("Change Shadow", sub => sub.StyleSet[index].Shadow = value, sub => sub.StyleSet[index].Shadow = oldValue)))
                        RaisePropertyChanged(nameof(Shadow));
                }
            }

            public Color ShadowColor
            {
                get
                {
                    if(selectedStyle == null)
                        return default(Color);
                    return selectedStyle.ShadowColor;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.ShadowColor;
                    if(!parent.Document.TryDo(new DocumentAction("Change ShadowColor", sub => sub.StyleSet[index].ShadowColor = value, sub => sub.StyleSet[index].ShadowColor = oldValue)))
                        RaisePropertyChanged(nameof(ShadowColor));
                }
            }

            public double Spacing
            {
                get
                {
                    if(selectedStyle == null)
                        return default(double);
                    return selectedStyle.Spacing;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Spacing;
                    if(!parent.Document.TryDo(new DocumentAction("Change Spacing", sub => sub.StyleSet[index].Spacing = value, sub => sub.StyleSet[index].Spacing = oldValue)))
                        RaisePropertyChanged(nameof(Spacing));
                }
            }

            public bool Strikeout
            {
                get
                {
                    if(selectedStyle == null)
                        return default(bool);
                    return selectedStyle.Strikeout;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Strikeout;
                    if(!parent.Document.TryDo(new DocumentAction("Change Strikeout", sub => sub.StyleSet[index].Strikeout = value, sub => sub.StyleSet[index].Strikeout = oldValue)))
                        RaisePropertyChanged(nameof(Strikeout));
                }
            }

            public bool Underline
            {
                get
                {
                    if(selectedStyle == null)
                        return default(bool);
                    return selectedStyle.Underline;
                }
                set
                {
                    if(selectedStyle == null)
                        return;
                    var index = selectedIndex;
                    var oldValue = selectedStyle.Underline;
                    if(!parent.Document.TryDo(new DocumentAction("Change Underline", sub => sub.StyleSet[index].Underline = value, sub => sub.StyleSet[index].Underline = oldValue)))
                        RaisePropertyChanged(nameof(Underline));
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
            if(SelectedStyleEditor != null)
                SelectedStyleEditor.Cleanup();
            SelectedStyleEditor = null;
            styles = null;
            selected = null;
            base.Cleanup();
        }
    }
}
