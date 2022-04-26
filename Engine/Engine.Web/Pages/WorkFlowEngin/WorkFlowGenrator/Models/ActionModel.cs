using Caspian.Engine;
using Newtonsoft.Json;

namespace Main.Models
{
    public class ActionModel
    {
        public ActionModel()
        {
            
        }

        public ActionModel(Caspian.Engine.Action action)
        {
            Namespace = action.Namespace;
            ClassName = action.ClassName ;
            MethodName = action.MethodName;
            ActionType = action.SystemActionType.Value;
        }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("actionType")]
        public SystemActionType ActionType { get; set; }

        [JsonProperty("activity")]
        public Activity Activity { get; set; }

        public Caspian.Engine.Action GetAction()
        {
            return new Caspian.Engine.Action()
            {
                Namespace = Namespace,
                ClassName = ClassName,
                MethodName = MethodName,
                SystemActionType = ActionType
            };
        }
    }

    public enum WorkflowWindowType
    {
        ActivityProperty,

        DynamicFields,

        ActivityField,

        CheckMethod,

        /// <summary>
        /// ارجاع توسط کاربر و یا براساس داده های مدل
        /// </summary>
        UserAndProperyRefrence,

        /// <summary>
        /// ارجاع براساس خروجی متد
        /// </summary>
        MethodReturnRefrence,

        ProcessMethod
    }
}