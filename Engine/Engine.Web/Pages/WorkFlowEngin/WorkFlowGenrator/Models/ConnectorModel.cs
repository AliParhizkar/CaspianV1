using System;
using Caspian.Engine;
using Newtonsoft.Json;
using System.ComponentModel;
using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Main.Models
{
    public class ConnectorModel
    {
        public static string GetPortName(ConnectorPortType portType)
        {
            switch(portType)
            {
                case ConnectorPortType.Bottom: return "B";
                case ConnectorPortType.left: return "L";
                case ConnectorPortType.Right: return "R";
                case ConnectorPortType.Top: return "T";
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public ConnectorModel()
        {

        }

        public ConnectorModel(Connector connector)
        {
            Id = connector.Id.ToString();
            From = connector.ActivityId.ToString();
            To = connector.ToActivityId.ToString();
            FromPort = ConnectorModel.GetPortName(connector.PortType);
            ToPort = ConnectorModel.GetPortName(connector.ToPortType);
            Title = connector.Title;
            FieldName = connector.FieldName;
            CompareType = connector.CompareType;
            Value = connector.Value;
            CheckValidation = connector.CheckValidation == true;
        }

        [JsonProperty("key")]
        public string Id { get; set; }

        [JsonProperty("text"), DisplayName("عنوان")]
        public string Title { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("fromPort")]
        public string FromPort { get; set; }

        [JsonProperty("toPort")]
        public string ToPort { get; set; }

        [JsonProperty(IsReference = false)]
        public bool Selected { get; set; }

        [JsonProperty("fieldValue")]
        public string EnTitle { get; set; }

        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("compareType"), DisplayName("نوع مقایسه")]
        public CompareType? CompareType { get; set; }

        [JsonProperty("value"), DisplayName("ارزش")]
        public decimal? Value { get; set; }

        [JsonProperty("fromActivity")]
        public ActivityModel FromActivity { get; set; }

        [JsonProperty("toActivity")]
        public ActivityModel ToActivity { get; set; }

        [JsonProperty("checkValidation")]
        public bool CheckValidation { get; set; }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Connector GetConnector()
        {
            var connector = new Connector();
            connector.ActivityId = Convert.ToInt32(From);
            connector.ToActivityId = Convert.ToInt32(To);
            connector.PortType = GetPortType(FromPort);
            connector.ToPortType = GetPortType(ToPort);
            connector.Title = Title;
            connector.CompareType = this.CompareType;
            connector.FieldName = FieldName;
            connector.Value = Value;
            connector.CheckValidation = CheckValidation;
            return connector;
        }

        public ConnectorPortType GetPortType(string name)
        {
            switch(name)
            {
                case "B": return ConnectorPortType.Bottom;
                case "L": return ConnectorPortType.left;
                case "R": return ConnectorPortType.Right;
                case "T": return ConnectorPortType.Top;
                default:
                    throw new NotImplementedException("عدم پیاده سازی");
            }
        }
    }

    public enum ConnectorType
    {
        [Display(Name = "کاربری")]
        UserDefined = 1,

        [Display(Name = "سیستمی")]
        Systemic
    }
}