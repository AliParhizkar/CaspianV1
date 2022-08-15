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
            switch(activity.ActivityType)
            {
                case ActivityType.Start: Category = "Start";break;
                case ActivityType.User: Category = "";break;
                case ActivityType.Validator: Category = "Validator"; break;
                case ActivityType.End: Category = "End"; break;
                case ActivityType.Comment: Category = "Comment"; break;
                case ActivityType.Parallelogram1: Category = "Parallelogram1";break;
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            Location = activity.Left + " " + activity.Top;
            Text = activity.Title;
            var actions = activity.OutConnectors.Select(t => t.Title);
            ActorType = activity.ActorType;
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

        public Activity GetActivity()
        {
            var activity = new Activity();
            switch(Category)
            {
                case "Start":
                    activity.ActivityType = ActivityType.Start;
                    break;
                case "":
                case null:
                    activity.ActivityType = ActivityType.User;
                    break;
                case "Validator":
                    activity.ActivityType = ActivityType.Validator;
                    break;
                case "End":
                    activity.ActivityType = ActivityType.End;
                    break;
                case "Comment":
                    activity.ActivityType = ActivityType.Comment;
                    break;
                case "Parallelogram1":
                    activity.ActivityType = ActivityType.Parallelogram1;
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
            return activity;
        }
    }
}