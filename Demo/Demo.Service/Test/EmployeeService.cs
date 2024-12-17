using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class EmployeeService: BaseService<Employee>, IBaseService<Employee>
    {
        public EmployeeService(IServiceProvider provider):
            base(provider) 
        {
            RuleFor(t => t.FName).Required();
            RuleFor(t => t.LName).Required();
            RuleFor(t => t.IdCard).Required().UniqueAsync("کاربرای با این کد ملی در سیستم وجود دارد.").CheckIdCard();
            RuleFor(t => t.EmploymentNo).Required().UniqueAsync("کارمندی با این کد در سیستم تعریف شده است");
        }
    }

    public class CourseStudyService : BaseService<CourseStudy>, IBaseService<CourseStudy>
    {
        public CourseStudyService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("پایه تحصیلی با این عنوان در سیستم تعریف شده است");
        }
    }

    public class FamilyService : BaseService<Family>, IBaseService<Family>
    {
        public FamilyService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.WifeName).Required();
        }
    }

    public class AddressService : BaseService<Address>, IBaseService<Address>
    {
        public AddressService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.AddressName).Required();
        }
    }

    public class ReligionAndSubReligionService : BaseService<ReligionAndSubReligion>, IBaseService<ReligionAndSubReligion>
    {
        public ReligionAndSubReligionService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.ReligionId).Required();
            RuleFor(t => t.SubReligionId).CustomAsync(async t =>
            {
                if (t.SubReligionId.HasValue)
                {
                    var flag = await provider.GetCaspianService<SubReligionService>().AnyAsync(u => u.Id == t.SubReligionId && 
                            u.ReligionId == t.ReligionId);
                    return !flag;
                }
                return false;
            }, "مقادیر دین و مذهب درست مقداردهی نشده اند");
        }
    }

    public class SubReligionService : BaseService<SubReligion>, IBaseService<SubReligion>
    {
        public SubReligionService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.ReligionId, "این مورد تکراری است");
        }
    }

    public class IdentificationDetailService: BaseService<IdentificationDetail>, IBaseService<IdentificationDetail>
    {
        public IdentificationDetailService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.BirthCityId).CheckCascadeAsync(t => t.BirthProvinceId, t => t.BirthCountryId, t => t.BirthCity.Province.CountryId, "شهر، استان و کشور محل تولد درست انتخاب نشده اند");
            RuleFor(t => t.RegCityId).CheckCascadeAsync(t => t.RegProvinceId, t => t.RegCountryId, t => t.RegCity.CountryId, "شهر، استان و کشور محل ثبت شناسنامه درست انتخاب نشده اند");
            RuleFor(t => t.BirthProvinceId).CheckCascadeAsync(t => t.BirthCountryId, "استان و کشور محل تولد درست انتخاب نشده اند");
            RuleFor(t => t.RegProvinceId).CheckCascadeAsync(t => t.RegCountryId, "استان و کشور محل ثبت شناسنامه درست انتخاب نشده اند");
        }
    }
}
