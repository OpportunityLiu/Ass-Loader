using SharpDX.DirectWrite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;

namespace SubtitleEditor.Model
{
    internal sealed class FontList : IReadOnlyList<Windows.UI.Xaml.Media.FontFamily>
    {
        public static FontList Instance => LazyInitializer.EnsureInitialized(ref ins, () => new FontList());

        public int Count => this.fonts.Count;

        public Windows.UI.Xaml.Media.FontFamily this[int index] => this.fonts[index];

        private static FontList ins;

        private FontList()
        {
            this.Refresh();
        }

        public Windows.UI.Xaml.Media.FontFamily GetFamily(string facename)
        {
            using (var factory = new Factory().QueryInterface<Factory5>())
            using (var introp = factory.GdiInterop)
            {
                try
                {
                    var font = introp.FromLogFont(new GdiInterop.LogFont
                    {
                        lfCharSet = 1,
                        lfFaceName = facename
                    });
                    return new Windows.UI.Xaml.Media.FontFamily(font.FontFamily.FamilyNames.GetString(0));
                }
                catch (Exception)
                {
                    return new Windows.UI.Xaml.Media.FontFamily(facename);
                }
            }
        }

        public void Refresh()
        {
            var localNames = new List<string>();
            foreach (var langCode in ApplicationLanguages.Languages)
            {
                var culture = CultureInfo.GetCultureInfo(langCode);
                while (!string.IsNullOrEmpty(culture?.Name))
                {
                    localNames.Add(culture.Name);
                    culture = culture.Parent;
                }
            }
            using (var factory = new Factory())
            using (var fontCollection = factory.GetSystemFontCollection(true))
            {
                var fonts = new List<string>();
                var count = fontCollection.FontFamilyCount;
                for (var i = 0; i < count; i++)
                {
                    using (var font = fontCollection.GetFontFamily(i))
                    {
                        using (var names = font.FamilyNames)
                        {
                            var lcc = names.Count;
                            var lnames = Enumerable.Range(0, lcc).Select(ii => names.GetString(ii)).ToArray();
                            var lloccals = Enumerable.Range(0, lcc).Select(ii => names.GetLocaleName(ii)).ToArray();
                            var index = 0;
                            foreach (var localName in localNames)
                            {
                                if (names.FindLocaleName(localName, out index))
                                    break;
                            }
                            if (index <= 0)
                                fonts.Add(names.GetString(0));
                            else
                                fonts.Add(names.GetString(index));
                        }
                    }
                }
                this.fonts = fonts.OrderBy(s => s).Distinct().Select(name => new Windows.UI.Xaml.Media.FontFamily(name)).ToList();
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
