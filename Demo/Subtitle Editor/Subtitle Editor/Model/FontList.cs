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

        public void Refresh()
        {
            var localNames = new List<string>();
            {
                var culture = CultureInfo.CurrentUICulture;
                while (!string.IsNullOrEmpty(culture?.Name))
                {
                    localNames.Add(culture.Name);
                    culture = culture.Parent;
                }
            }
            localNames.AddRange(ApplicationLanguages.Languages);
            this.gdiFontFaces = new List<GdiInterop.LogFont>();
            using (var factory = new Factory())
            using (var gdiInterop = factory.GdiInterop)
            using (var fontCollection = factory.GetSystemFontCollection(true))
            {
                var fonts = new List<string>();
                var count = fontCollection.FontFamilyCount;
                for (var i = 0; i < count; i++)
                {
                    using (var font = fontCollection.GetFontFamily(i))
                    {
                        var faceCount = font.FontCount;
                        for (var j = 0; j < faceCount; j++)
                        {
                            try
                            {
                                using (var face = font.GetFont(j))
                                {
                                    var logF = new GdiInterop.LogFont();
                                    if (gdiInterop.ToLogFont(face, logF))
                                    {
                                        fonts.Add(logF.lfFaceName);
                                        this.gdiFontFaces.Add(logF);
                                    }
                                }

                            }
                            catch (Exception)
                            {
                            }
                        }
                        //string familyName;
                        //using(var names = font.FamilyNames)
                        //{
                        //    var index = 0;
                        //    foreach(var localName in localNames)
                        //    {
                        //        if(names.FindLocaleName(localName, out index))
                        //            break;
                        //    }
                        //    if(index <= 0)
                        //        familyName = names.GetString(0);
                        //    else
                        //        familyName = names.GetString(index);
                        //}
                    }
                }
                this.fonts = fonts.OrderBy(s => s).Distinct().Select(name => new Windows.UI.Xaml.Media.FontFamily(name)).ToList();
            }
        }

        private List<GdiInterop.LogFont> gdiFontFaces;

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
