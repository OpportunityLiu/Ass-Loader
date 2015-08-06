using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SubtitleEditor.Model
{
    [JsonObject]
    class ProjectConfig
    {
        [JsonProperty]
        public string VideoPath
        {
            get;
            set;
        }

        [JsonProperty]
        public string AudioPath
        {
            get;
            set;
        }
    }
}
