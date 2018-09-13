using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using Opportunity.MvvmUniverse.Collections;
using Opportunity.MvvmUniverse.Commands;
using Opportunity.MvvmUniverse.Views;
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

        public Model.FontList FontList => Model.FontList.Instance;

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Subtitle))
            {
                this.SelectedStyle = null;
                this.Styles.Update(this.Document.Subtitle?.Styles ?? (IReadOnlyList<Style>)Array.Empty<Style>());
            }
        }

        public ObservableList<Style> Styles { get; } = new ObservableList<Style>();

        public Style SelectedStyle
        {
            get => this.selected;
            set
            {
                if (Set(ref this.selected, value))
                    this.SelectedStyleEditor.SelectedStyle = value;
            }
        }

        private Style selected;

        public Command<Style> Add => Commands.GetOrAdd(() => Command<Style>.Create((c, style) =>
        {
            this.Document.Do(new DocumentAction("Add style.", sub => sub.Styles.Add(style), sub => sub.Styles.Remove(style)));
        }, (c, style) => style != null && !this.Document.Subtitle.Styles.ContainsName(style.Name)));

        public Command<Style> Remove => Commands.GetOrAdd(() => Command<Style>.Create((c, style) =>
        {
            var i = this.Styles.IndexOf(style);
            if (i < 0)
                return;
            this.Document.Do(new DocumentAction("Remove style.", sub =>
            {
                this.Styles.Remove(style);
                sub.Styles.Remove(style);
            }, sub =>
            {
                this.Styles.Insert(i, style);
                sub.Styles.Insert(i, style);
            }));
        }, (c, style) => style != null && this.Document.Subtitle.Styles.Contains(style)));

        public class SelectedStyleViewModel : ViewModelBase
        {
            private static readonly Style defaultStyle = new Style("*Default");

            private readonly StyleViewModel parent;

            private Style selectedStyle;
            public Style SelectedStyle
            {
                get => this.selectedStyle;
                set
                {
                    if (Set(ref this.selectedStyle, value))
                    {
                        this.selectedIndex = this.parent.Styles.IndexOf(this.selectedStyle);
                        this.Rename.OnCanExecuteChanged();
                        //FIX:
                        this.OnPropertyChanged(new PropertyChangedEventArgs(""));
                    }
                }
            }
            private int selectedIndex;

            public SelectedStyleViewModel(StyleViewModel parent)
            {
                this.parent = parent;
                this.SelectedStyle = parent.SelectedStyle;
            }

            public Command<string> Rename => Commands.GetOrAdd(() => Command<string>.Create((c, newName) =>
            {
                var index = this.selectedIndex;
                var aName = newName;
                var addIndex = 2;
                while (this.parent.Document.Subtitle.Styles.ContainsName(aName))
                    aName = $"{newName} ({addIndex++})";
                var oldValue = this.selectedStyle;
                var newValue = oldValue.Clone(aName);
                this.parent.Document.Do(new DocumentAction("Change Name", sub =>
                {
                    var temp = this.parent.SelectedStyle == oldValue;
                    sub.Styles[index] = newValue;
                    if (temp)
                        this.parent.SelectedStyle = newValue;
                    this.OnPropertyChanged(nameof(Name));
                }, sub =>
                {
                    var temp = this.parent.SelectedStyle == newValue;
                    sub.Styles[index] = oldValue;
                    if (temp)
                        this.parent.SelectedStyle = oldValue;
                    this.OnPropertyChanged(nameof(Name));
                }));
            }, (c, newName) => this.selectedStyle != null && !newName.IsNullOrEmpty()));

            public string Name => (this.selectedStyle ?? defaultStyle).Name;

            public AlignmentStyle Alignment
            {
                get => (this.selectedStyle ?? defaultStyle).Alignment;
                set
                {
                    if (this.selectedStyle is null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.Alignment;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Alignment", sub => sub.Styles[index].Alignment = value, sub => sub.Styles[index].Alignment = oldValue)))
                        this.OnPropertyChanged(nameof(this.Alignment));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Bold", sub => sub.Styles[index].Bold = value, sub => sub.Styles[index].Bold = oldValue)))
                        this.OnPropertyChanged(nameof(this.Bold));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change BorderStyle", sub => sub.Styles[index].BorderStyle = value, sub => sub.Styles[index].BorderStyle = oldValue)))
                        this.OnPropertyChanged(nameof(this.BorderStyle));
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
            //        if(!parent.Document.TryDo(new DocumentAction("Change Encoding", sub => sub.Styles[index].Encoding = value, sub => sub.Styles[index].Encoding = oldValue)))
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change FontName", sub => sub.Styles[index].FontName = value, sub => sub.Styles[index].FontName = oldValue)))
                        this.OnPropertyChanged(nameof(this.FontName));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change FontSize", sub => sub.Styles[index].FontSize = value, sub => sub.Styles[index].FontSize = oldValue)))
                        this.OnPropertyChanged(nameof(this.FontSize));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Italic", sub => sub.Styles[index].Italic = value, sub => sub.Styles[index].Italic = oldValue)))
                        this.OnPropertyChanged(nameof(this.Italic));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginL", sub => sub.Styles[index].MarginL = value, sub => sub.Styles[index].MarginL = oldValue)))
                        this.OnPropertyChanged(nameof(this.MarginL));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginR", sub => sub.Styles[index].MarginR = value, sub => sub.Styles[index].MarginR = oldValue)))
                        this.OnPropertyChanged(nameof(this.MarginR));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change MarginV", sub => sub.Styles[index].MarginV = value, sub => sub.Styles[index].MarginV = oldValue)))
                        this.OnPropertyChanged(nameof(this.MarginV));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Outline", sub => sub.Styles[index].Outline = value, sub => sub.Styles[index].Outline = oldValue)))
                        this.OnPropertyChanged(nameof(this.Outline));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change OutlineColor", sub => sub.Styles[index].OutlineColor = value, sub => sub.Styles[index].OutlineColor = oldValue)))
                        this.OnPropertyChanged(nameof(this.OutlineColor));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change PrimaryColor", sub => sub.Styles[index].PrimaryColor = value, sub => sub.Styles[index].PrimaryColor = oldValue)))
                        this.OnPropertyChanged(nameof(this.PrimaryColor));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Rotation", sub => sub.Styles[index].Rotation = value, sub => sub.Styles[index].Rotation = oldValue)))
                        this.OnPropertyChanged(nameof(this.Rotation));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ScaleX", sub => sub.Styles[index].ScaleX = value, sub => sub.Styles[index].ScaleX = oldValue)))
                        this.OnPropertyChanged(nameof(this.ScaleX));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ScaleY", sub => sub.Styles[index].ScaleY = value, sub => sub.Styles[index].ScaleY = oldValue)))
                        this.OnPropertyChanged(nameof(this.ScaleY));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change SecondaryColor", sub => sub.Styles[index].SecondaryColor = value, sub => sub.Styles[index].SecondaryColor = oldValue)))
                        this.OnPropertyChanged(nameof(this.SecondaryColor));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Shadow", sub => sub.Styles[index].Shadow = value, sub => sub.Styles[index].Shadow = oldValue)))
                        this.OnPropertyChanged(nameof(this.Shadow));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change ShadowColor", sub => sub.Styles[index].ShadowColor = value, sub => sub.Styles[index].ShadowColor = oldValue)))
                        this.OnPropertyChanged(nameof(this.ShadowColor));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Spacing", sub => sub.Styles[index].Spacing = value, sub => sub.Styles[index].Spacing = oldValue)))
                        this.OnPropertyChanged(nameof(this.Spacing));
                }
            }

            public bool StrikeOut
            {
                get => (this.selectedStyle ?? defaultStyle).StrikeOut;
                set
                {
                    if (this.selectedStyle == null)
                        return;
                    var index = this.selectedIndex;
                    var oldValue = this.selectedStyle.StrikeOut;
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Strikeout", sub => sub.Styles[index].StrikeOut = value, sub => sub.Styles[index].StrikeOut = oldValue)))
                        this.OnPropertyChanged(nameof(this.StrikeOut));
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
                    if (!this.parent.Document.TryDo(new DocumentAction("Change Underline", sub => sub.Styles[index].Underline = value, sub => sub.Styles[index].Underline = oldValue)))
                        this.OnPropertyChanged(nameof(this.Underline));
                }
            }
        }

        public SelectedStyleViewModel SelectedStyleEditor { get; }
    }
}
