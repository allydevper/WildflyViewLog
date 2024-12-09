using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildflyViewLog.Models
{
    public class Labels
    {
        [JsonProperty("agent.googleapis.com/log_file_path")]
        public string FilePath { get; set; }
    }

    public class JsonPayload
    {
        public string Message { get; set; }
    }

    public class WildflyData
    {
        public JsonPayload JsonPayload { get; set; }
        public Labels Labels { get; set; }
    }
}
