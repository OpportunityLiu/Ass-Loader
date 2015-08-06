using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader;
using AssLoader.Collections;

namespace SubtitleEditor.Model
{
    class ScriptInfo : AssScriptInfo
    {
        [ScriptInfo(nameof(SubtitleEditorConfig), IsOptional = true)]
        [JsonSerializer(typeof(ProjectConfig))]
        private ProjectConfig subtitleEditorConfig;

        public ProjectConfig SubtitleEditorConfig
        {
            get
            {
                return subtitleEditorConfig;
            }
            set
            {
                Set(ref subtitleEditorConfig, value);
            }
        }
    }
}
