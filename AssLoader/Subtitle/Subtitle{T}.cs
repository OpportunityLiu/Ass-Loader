using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace AssLoader
{
    public class Subtitle<TScriptInfo> where TScriptInfo : ScriptInfoCollection, new()
    {
        public Subtitle()
        {
            EventCollection = new Collections.EventCollection();
            StyleDictionary = new Collections.StyleDictionary();
            this.ScriptInfo = new TScriptInfo();
        }

        public void Serialize(TextWriter writer)
        {
            ThrowHelper.ThrowIfNull(writer, "writer");

            writer.WriteLine("[Script Info]");
            foreach(var line in Subtitle.EditorInfo)
            {
                writer.WriteLine(line);
            }
            this.ScriptInfo.ToString(writer);
            writer.WriteLine();

            writer.WriteLine("[V4+ Styles]");
            saveStyle(writer);
            writer.WriteLine();

            writer.WriteLine("[Events]");
            saveEvent(writer);
            writer.WriteLine();

            writer.Flush();
        }

        private void saveStyle(TextWriter writer)
        {
            writer.WriteLine(Subtitle.StyleFormat);
            foreach(var item in StyleDictionary.Values)
                writer.WriteLine(item.Serialize(Subtitle.StyleFormat));
        }

        private void saveEvent(TextWriter writer)
        {
            writer.WriteLine(Subtitle.EventFormat);
            foreach(var item in EventCollection)
                writer.WriteLine(item.Serialize(Subtitle.EventFormat));
        }

        public string Serialize()
        {
            using(var writer = new StringWriter(FormatHelper.DefaultFormat))
            {
                this.Serialize(writer);
                return writer.ToString();
            }
        }

        public TScriptInfo ScriptInfo
        {
            get;
            private set;
        }

        public Collections.StyleDictionary StyleDictionary
        {
            get;
            private set;
        }

        public Collections.EventCollection EventCollection
        {
            get;
            private set;
        }
    }
}
