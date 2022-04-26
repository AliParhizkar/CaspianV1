using Newtonsoft.Json;
using System.Collections.Generic;

namespace Main.Models
{
    public class WorkFlowModel
    {
        [JsonProperty("class")]
        public string Class { get { return "go.GraphLinksModel"; } }

        [JsonProperty("linkFromPortIdProperty")]
        public string LinkFromPortIdProperty { get { return "fromPort"; } }

        [JsonProperty("linkToPortIdProperty")]
        public string LinkToPortIdProperty { get { return "toPort"; } }

        [JsonProperty("nodeDataArray")]
        public IList<ActivityModel> Activities { get; set; }

        [JsonProperty("linkDataArray")]
        public IList<ConnectorModel> Connectors { get; set; }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}