using Caspian.Engine;
using Newtonsoft.Json;

namespace Main.Models
{
    public class ActivityDynamicFieldModel
    {
        public ActivityDynamicFieldModel()
        {
            
        }

        public ActivityDynamicFieldModel(ActivityDynamicField activityField)
        {
            FormId = activityField.FormId;
            ControlId = activityField.ControlId;
            ShowTime = activityField.ShowTime;
            ShowType = activityField.ShowType;
            Title = activityField.Title;
        }

        [JsonProperty("formId")]
        public int FormId { get; set; }

        [JsonProperty("controlId")]
        public int ControlId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("showType")]
        public ShowType? ShowType { get; set; }

        [JsonProperty("showTime")]
        public ShowTime? ShowTime { get; set; }

        public ActivityDynamicField GetActivityField()
        {
            return new ActivityDynamicField()
            {
                ControlId = ControlId,
                FormId = FormId,
                ShowTime = ShowTime,
                ShowType = ShowType,
                Title = Title
            };
        }
    }
}