using System;
using System.Linq;
using Caspian.Engine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Main.Models
{
    public class ActivityModel
    {
        public ActivityModel()
        {

        }

        public ActivityModel(Activity activity)
        {
            Id = activity.Id.ToString();
            switch(activity.CategoryType)
            {
                case CategoryType.Start: Category = "Start";break;
                case CategoryType.Default: Category = "";break;
                case CategoryType.Diamond: Category = "Diamond"; break;
                case CategoryType.End: Category = "End"; break;
                case CategoryType.Comment: Category = "Comment"; break;
                case CategoryType.Parallelogram1: Category = "Parallelogram1";break;
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            Location = activity.Left + " " + activity.Top;
            Text = activity.Title;
            var actions = activity.OutConnectors.Select(t => t.Title);
            if (activity.Action.ClassName != null)
                Action = new ActionModel(activity.Action);
            if (activity.Fields.Count > 0)
                Fields = activity.Fields.Select(t => new ActivityFieldModel(t)).ToList();
            ActorType = activity.ActorType;
            if (activity.DynamicFields.Count > 0)
                DynamicFields = activity.DynamicFields.Select(t => new ActivityDynamicFieldModel(t)).ToList();
        }

        [JsonProperty("workflowId")]
        public int WorkflowId { get; set; }

        [JsonProperty("key")]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("loc")]
        public string Location { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("actorType")]
        public ActorType? ActorType { get; set; }

        [JsonProperty("action")]
        public ActionModel Action { get; set; }

        [JsonProperty("fields")]
        public IList<ActivityFieldModel> Fields { get; set; }

        [JsonProperty("dynamicFields")]
        public IList<ActivityDynamicFieldModel> DynamicFields { get; set; }

        public Activity GetActivity()
        {
            var activity = new Activity();
            switch(Category)
            {
                case "Start":
                    activity.CategoryType = CategoryType.Start;
                    break;
                case "":
                case null:
                    activity.CategoryType = CategoryType.Default;
                    break;
                case "Diamond":
                    activity.CategoryType = CategoryType.Diamond;
                    break;
                case "End":
                    activity.CategoryType = CategoryType.End;
                    break;
                case "Comment":
                    activity.CategoryType = CategoryType.Comment;
                    break;
                case "Parallelogram1":
                    activity.CategoryType = CategoryType.Parallelogram1;
                    break;
                default:
                    throw new NotImplementedException("عدم پیاده سازی");
            }
            activity.Id = Convert.ToInt32(Id);
            var array = Location.Split(' ');
            activity.Left = Convert.ToInt32(Math.Floor(Convert.ToDouble(array[0])));
            activity.Top = Convert.ToInt32(Math.Floor(Convert.ToDouble(array[1])));
            activity.Title = Text;
            activity.ActorType = ActorType;
            if (Action != null)
                activity.Action = Action.GetAction();
            if (Fields != null)
                activity.Fields = Fields.Select(t => t.GetActivityField()).ToList();
            if (DynamicFields != null)
                activity.DynamicFields = DynamicFields.Select(t => t.GetActivityField()).ToList();
            return activity;
        }
    }
}