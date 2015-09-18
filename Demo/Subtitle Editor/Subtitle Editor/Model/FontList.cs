using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SharpDX.DirectWrite;
using System.Collections;
using System.Globalization;
using Windows.Globalization;

namespace SubtitleEditor.Model
{
    sealed class FontList : IReadOnlyList<Windows.UI.Xaml.Media.FontFamily>
    {
        public static FontList Instance => LazyInitializer.EnsureInitialized(ref ins, () => new FontList());

        public int Count
        {
            get
            {
                return this.fonts.Count;
            }
        }

        public Windows.UI.Xaml.Media.FontFamily this[int index]
        {
            get
            {
                return this.fonts[index];
            }
        }

        private static FontList ins;

        private FontList()
        {
            Refresh();
        }

        public void Refresh()
        {
            var localNames = new List<string>();
            {
                var culture = CultureInfo.CurrentUICulture;
                while(!string.IsNullOrEmpty(culture?.Name))
                {
                    localNames.Add(culture.Name);
                    culture = culture.Parent;
                }
            }
            localNames.AddRange(ApplicationLanguages.Languages);
            var x = new Language("zh-cn");
            using(var factory = new Factory())
            using(var fontCollection = factory.GetSystemFontCollection(true))
            {
                var fonts = new List<string>();
                var count = fontCollection.FontFamilyCount;
                for(int i = 0; i < count; i++)
                {
                    using(var font = fontCollection.GetFontFamily(i))
                    using(var names = font.FamilyNames)
                    {
                        var index = 0;
                        foreach(var localName in localNames)
                        {
                            if(names.FindLocaleName(localName, out index))
                                break;
                        }
                        if(index <= 0)
                            fonts.Add(names.GetString(0));
                        else
                            fonts.Add(names.GetString(index));
                    }
                }
                fonts.Sort(StringComparer.CurrentCultureIgnoreCase);
                this.fonts = fonts.Select(name => new Windows.UI.Xaml.Media.FontFamily(name)).ToList();
            }
        }

        private List<Windows.UI.Xaml.Media.FontFamily> fonts;

        public List<Windows.UI.Xaml.Media.FontFamily>.Enumerator GetEnumerator()
        {
            return this.fonts.GetEnumerator();
        }

        IEnumerator<Windows.UI.Xaml.Media.FontFamily> IEnumerable<Windows.UI.Xaml.Media.FontFamily>.GetEnumerator()
        {
            return this.fonts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.fonts.GetEnumerator();
        }
    }
}
