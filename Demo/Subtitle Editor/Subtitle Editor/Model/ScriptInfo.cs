using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.Model
{
    internal sealed class ScriptInfo : AssScriptInfo
    {
        [ScriptInfo(nameof(SubtitleEditorConfig), IsOptional = true)]
        [JsonSerializer(typeof(ProjectConfig))]
        public ProjectConfig SubtitleEditorConfig { get; set; }
    }
}
