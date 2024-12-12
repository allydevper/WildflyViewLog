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
        public required string FilePath { get; set; }
    }

    public class JsonPayload
    {
        public required string Message { get; set; }
    }

    public class WildflyData
    {
        public required JsonPayload JsonPayload { get; set; }
        public required Labels Labels { get; set; }
    }
}
