using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DataParameterValueService : SimpleService<DataParameterValue>, ISimpleService<DataParameterValue>
    {
        public DataParameterValueService(IServiceProvider provider)
            :base(provider)
        {
            var message1 = "مقدار این فیلد باید مشخص باشد";
            RuleFor(t => t.Value1).CustomValue(t => t <= 0, message1);
            RuleFor(t => t.Value2).Custom(t => t.Parameter2Id.HasValue && t.Value2 == null, message1);
            RuleFor(t => t.Value3).Custom(t => t.Parameter3Id.HasValue && t.Value3 == null, message1);
            RuleFor(t => t.Value4).Custom(t => t.Parameter4Id.HasValue && t.Value4 == null, message1);
            RuleFor(t => t.Value5).Custom(t => t.Parameter5Id.HasValue && t.Value5 == null, message1);
            RuleFor(t => t.Value6).Custom(t => t.Parameter6Id.HasValue && t.Value6 == null, message1);
            var message2 = "این حالت تکراری است";
            RuleFor(t => t.Parameter1Id).Custom(t => 
            { 
                if (t.Parameter2Id == null)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1);
                return false;
            }, message2);

            RuleFor(t => t.Parameter2Id).Custom(t =>
            {
                if (t.Parameter2Id.HasValue && t.Parameter3Id == null)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1 &&
                        u.Parameter2Id == t.Parameter2Id && u.Value2 == t.Value2);
                return false;
            }, message2);

            RuleFor(t => t.Parameter3Id).Custom(t =>
            {
                if (t.Parameter3Id.HasValue && t.Parameter4Id == null)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1 &&
                        u.Parameter2Id == t.Parameter2Id && u.Value2 == t.Value2 && u.Parameter3Id == t.Parameter3Id && u.Value3 == t.Value3);
                return false;
            }, message2);

            RuleFor(t => t.Parameter4Id).Custom(t =>
            {
                if (t.Parameter4Id.HasValue && t.Parameter5Id == null)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1 &&
                        u.Parameter2Id == t.Parameter2Id && u.Value2 == t.Value2 && u.Parameter3Id == t.Parameter3Id && u.Value3 == t.Value3 &&
                        u.Parameter4Id == t.Parameter4Id && u.Value4 == t.Value4);
                return false;
            }, message2);

            RuleFor(t => t.Parameter5Id).Custom(t =>
            {
                if (t.Parameter5Id.HasValue && t.Parameter6Id == null)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1 &&
                        u.Parameter2Id == t.Parameter2Id && u.Value2 == t.Value2 && u.Parameter3Id == t.Parameter3Id && u.Value3 == t.Value3 &&
                        u.Parameter4Id == t.Parameter4Id && u.Value4 == t.Value4 && u.Parameter5Id == t.Parameter5Id && u.Value5 == t.Value5);
                return false;
            }, message2);
            RuleFor(t => t.Parameter5Id).Custom(t =>
            {
                if (t.Parameter6Id.HasValue)
                    return GetAll().Any(u => t.Id != u.Id && u.Parameter1Id == t.Parameter1Id && u.Value1 == t.Value1 &&
                        u.Parameter2Id == t.Parameter2Id && u.Value2 == t.Value2 && u.Parameter3Id == t.Parameter3Id && u.Value3 == t.Value3 &&
                        u.Parameter4Id == t.Parameter4Id && u.Value4 == t.Value4 && u.Parameter5Id == t.Parameter5Id && u.Value5 == t.Value5 &&
                        u.Parameter6Id == t.Parameter6Id && u.Value6 == t.Value6);
                return false;
            }, message2);
        }
    }
}
