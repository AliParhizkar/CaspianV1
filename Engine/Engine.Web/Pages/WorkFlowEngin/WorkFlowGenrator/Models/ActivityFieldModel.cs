using Caspian.Engine;
using Newtonsoft.Json;

namespace Main.Models
{
    public class ActivityFieldModel
    {
        public ActivityFieldModel()
        {

        }

        public ActivityFieldModel(ActivityField activityField)
        {
            FieldName = activityField.FieldName;
            ShowType = activityField.ShowType;
        }

        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("showType")]
        public ShowType ShowType { get; set; }

        public ActivityField GetActivityField()
        {
            return new ActivityField()
            {
                FieldName = FieldName,
                ShowType = this.ShowType
            };
        }
    }
}