using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.Model
{
    [JsonObject]
    internal class ProjectConfig
    {
        [JsonProperty]
        public string MediaToken { get; set; }
    }
}
