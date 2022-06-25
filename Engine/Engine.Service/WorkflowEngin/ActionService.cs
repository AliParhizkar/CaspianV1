//using Caspian.Common;
//using System.Reflection;
//using Caspian.Common.Service;
//using Caspian.Common.Extension;
//using Caspian.Common.Attributes;
//using Microsoft.Extensions.DependencyInjection;

//namespace Caspian.Engine.Service
//{
//    public class ActionService : SimpleService<Action>, ISimpleService<Action>
//    {
//        WorkflowService workflowService;
//        public ActionService(IServiceScope scope)
//            :base(scope)
//        {
//            workflowService = this.GetService<WorkflowService>();
//        }


//        /// <summary>
//        /// این متد تمامی اکشنهای گردش را حذف می کند
//        /// </summary>
//        /// <param name="workflowId"></param>
//        public void RemoverWorkflowActions(int workflowId)
//        {
//            var actions = GetAll().Where(t => t.Activity.WorkflowId == workflowId);
//            RemoveRange(actions); 
//        }

//        async public Task<IDictionary<string, string>> GetActionFields(int workflowId, Action action)
//        {
//            var workflow = await workflowService.SingleAsync(workflowId);
//            var types = workflow.FormGroup.Subsystem.GetServiceAssembly().GetTypes();
//            MethodInfo method = null;
//            foreach (var type in types)
//            {
//                method = type.GetMethods().SingleOrDefault(t => t.ReflectedType.Namespace == action.Namespace &&
//                    t.ReflectedType.Name == action.ClassName && t.Name == action.MethodName && 
//                    t.GetCustomAttribute<TaskAttribute>() != null);
//                if (method != null)
//                    break;
//            }
//            if (method == null)
//                throw new Exception("متد " + action.MethodName + " از سیستم حذف شده است.");
//            var returnType = method.ReturnType;
//            if (returnType.IsNullableType())
//                returnType = Nullable.GetUnderlyingType(returnType);
//            var dic = new Dictionary<string, string>();
//            if (returnType == typeof(bool))
//            {
//                dic.Add("true", "درست");
//                dic.Add("false", "نادرست");
//                return dic;
//            }
//            if (!returnType.IsEnum)
//                throw new Exception("خروجی متد " + action.MethodName + " باید ار نوع بولین یا شمارشی باشد");
//            foreach(var field in returnType.GetFields().Where(t => !t.IsSpecialName))
//            {
//                var attr = field.GetCustomAttribute<EnumFieldAttribute>();
//                if (attr == null)
//                    throw new Exception("نوع " + returnType.Name + " فیلد " + field.Name + " باید دارای EnumFieldAttribute باشد.");
//                dic.Add(field.Name, attr.DisplayName);
//            }
//            return dic;
//        }

//        async public Task<IEnumerable<Action>> GetProcessAction(int workflowId)
//        {
//            var workflow = await workflowService.SingleAsync(workflowId);
//            var formGroup = workflow.FormGroup;
//            var types = formGroup.Subsystem.GetServiceAssembly().GetTypes();
//            var list = new List<Action>();
//            foreach (var type in types)
//            {
//                foreach (var method in type.GetMethods())
//                {
//                    var returnType = method.ReturnType;
//                    if (returnType == typeof(void))
//                    {
//                        var taskAttribute = method.GetCustomAttribute<TaskAttribute>();
//                        if (taskAttribute != null)
//                        {
//                            var parameters = method.GetParameters();
//                            if (parameters.Count() == 1)
//                            {
//                                var parameterType = parameters[0].ParameterType;
//                                if (parameterType.Namespace == formGroup.Namespace && parameterType.Name == formGroup.ClassName)
//                                {
//                                    list.Add(new Action()
//                                    {
//                                        SystemActionType = SystemActionType.Checking,
//                                        Activity = new Activity()
//                                        {
//                                            Workflow = new Workflow()
//                                            {

//                                            }
//                                        },
//                                        SubSystemKind = formGroup.Subsystem,
//                                        Namespace = method.ReflectedType.Namespace,
//                                        ClassName = method.ReflectedType.Name,
//                                        MethodName = method.Name,
//                                        FaTitle = taskAttribute.Title
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return list;
//        }

//        async public Task<Type> GetCheckingActionReturnType(int workflowId, string @namespace, string className, string methodName)
//        {
//            var workflow = await workflowService.SingleAsync(workflowId);
//            var formGroup = workflow.FormGroup;
//            var type = formGroup.Subsystem.GetServiceAssembly().GetTypes().Single(t => t.Namespace == @namespace && t.Name == className);
//            foreach (var method in type.GetMethods().Where(t => t.Name == methodName))
//            {
//                if (method.GetCustomAttribute<TaskAttribute>() != null)
//                    return method.ReturnType;
//            }
//            throw new CaspianException("خطا: In type " + className + " method " + methodName + " not exist or hasnt Task Attribut");
//        }

//        async public Task<IEnumerable<Action>> GetCheckingAction(int workflowId)
//        {
//            var workflow = await workflowService.SingleAsync(workflowId);
//            var formGroup = workflow.FormGroup;
//            var types = formGroup.Subsystem.GetServiceAssembly().GetTypes();
//            var list = new List<Action>();
//            foreach(var type in types)
//            {
//                foreach(var method in type.GetMethods())
//                {
//                    var returnType = method.ReturnType;
//                    if (returnType.IsEnumType() || returnType == typeof(bool) || returnType == typeof(bool?))
//                    {
//                        var taskAttribute = method.GetCustomAttribute<TaskAttribute>();
//                        if (taskAttribute != null)
//                        {
//                            var parameters = method.GetParameters();
//                            if (parameters.Count() == 1)
//                            {
//                                var parameterType = parameters[0].ParameterType;
//                                if (parameterType.Namespace == formGroup.Namespace && parameterType.Name == formGroup.ClassName)
//                                {
//                                    list.Add(new Action()
//                                    {
//                                        SystemActionType = SystemActionType.Checking,
//                                        Activity = new Activity()
//                                        {
//                                            Workflow = new Workflow()
//                                            {

//                                            }
//                                        },
//                                        Namespace = method.ReflectedType.Namespace,
//                                        ClassName = method.ReflectedType.Name,
//                                        MethodName = method.Name,
//                                        FaTitle = taskAttribute.Title
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return list;
//        }

//        async public Task<IEnumerable<Action>> GetOperationAction(int workflowId)
//        {
//            var workflow = await workflowService.SingleAsync(workflowId);
//            var formGroup = workflow.FormGroup;
//            var types = formGroup.Subsystem.GetServiceAssembly().GetTypes();
//            var list = new List<Action>();
//            foreach (var type in types)
//            {
//                foreach (var method in type.GetMethods())
//                {
//                    var returnType = method.ReturnType;
//                    if (!returnType.IsEnumType() && returnType != typeof(bool) && returnType != typeof(bool?))
//                    {
//                        var taskAttribute = method.GetCustomAttribute<TaskAttribute>();
//                        if (taskAttribute != null)
//                        {
//                            var parameters = method.GetParameters();
//                            if (parameters.Count() == 1)
//                            {
//                                var parameterType = parameters[0].ParameterType;
//                                if (parameterType.Namespace == formGroup.Namespace && parameterType.Name == formGroup.ClassName)
//                                {
//                                    list.Add(new Action()
//                                    {
//                                        SystemActionType = SystemActionType.Operation,
//                                        Activity = new Activity()
//                                        {
//                                            Workflow = new Workflow()
//                                            {

//                                            }
//                                        },
//                                        Namespace = method.ReflectedType.Namespace,
//                                        ClassName = method.ReflectedType.Name,
//                                        MethodName = method.Name
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return list;
//        }

//        public decimal? ExecuteFunction(object data, Assembly assembly, Action action)
//        {
//            var type = assembly.GetTypes().Single(t => action.Namespace == t.Namespace &&
//            t.Name == action.ClassName);
//            var method =  type.GetMethod(action.MethodName);
//            var result = method.Invoke(Activator.CreateInstance(type), new object[] { data });
//            if (method.ReturnType != typeof(void) && result == null)
//                throw new Exception("خطا:Method " + action.MethodName + " in type " + action.ClassName +
//                    " return null that is invalid");
//            if (result == null)
//                return null;
//            return Convert.ToDecimal(result);
//        }
//    }
//}
