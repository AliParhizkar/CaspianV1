using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine
{
    public class ActorService 
    {
        public ActorService(IServiceScope scope)
        {

        }
        public int? GetActorId(int userId, SubSystemKind systemKind, Type genericType, Activity activity)
        {
            if (activity.ActivityType != ActivityType.User)
                return null;
            var types = systemKind.GetServiceAssembly().GetTypes();
            Type mainType = null;
            var interfaceType = typeof(IActor<>).MakeGenericType(genericType);
            foreach (var type in types)
            {
                if (type.GetInterfaces().Any(t => t == interfaceType))
                {
                    mainType = type;
                    break;
                }
            }
            if (mainType == null)
                throw new Exception("خطا:No type in " + systemKind.ToString() + " dont implements interface IActor<>");
            string methodName = null;
            switch(activity.ActorType)
            {
                case ActorType.Creator:
                    return userId;
                case ActorType.Master:
                    methodName = "GetMasterUserId";
                    break;
                case ActorType.MasterOfMaster:
                    methodName = "GetMasterOfMasterUserId";
                    break;
                default:
                    throw new Exception("خطای عدم پیاده سازی");
            }
            var actor = Activator.CreateInstance(mainType);
            return Convert.ToInt32(mainType.GetMethod(methodName).Invoke(actor, new object[] { userId}));
        }
    }
}
