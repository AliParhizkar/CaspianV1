using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service
{
    public class EmployeeService: BaseService<Employee>
    {
        public EmployeeService(IServiceProvider provider):
            base(provider) 
        {
            RuleFor(t => t.FName).Required();
            RuleFor(t => t.LName).Required();
            RuleFor(t => t.IdCard).Required().UniqueAsync("کاربرای با این کد ملی در سیستم وجود دارد.").CheckIdCard();
            RuleFor(t => t.EmploymentNo).Required().UniqueAsync("کارمندی با این کد در سیستم تعریف شده است");
            RuleFor(t => t.ScopeId).CustomAsync(async t => 
            {
                var result = await provider.GetCaspianService<SimpleDataService>().GetAll()
                    .AnyAsync(u => u.DataType == SimpleDataType.Scope && t.ScopeId == u.Id);
                return !result;
            }, "حوزه ای با این کد تعریف نشده است");
            RuleFor(t => t.CostCenterId).CustomAsync(async t =>
            {
                if (t.CostCenterId == null)
                    return false;
                var result = await provider.GetCaspianService<SimpleDataService>().GetAll()
                    .AnyAsync(u => u.DataType == SimpleDataType.CostCenter && t.CostCenterId == u.Id);
                return !result;
            }, "مرکز هزینه ای با این کد تعریف نشده است");
            RuleFor(t => t.Family).ChildRules(t =>
            {
                t.RuleFor(u => u.WifeName).Required();
                t.RuleFor(u => u.MariageDate).Required();
            });
            RuleFor(t => t.Address).ChildRules(t =>
            {
                t.RuleFor(u => u.AddressName).Required();
            });
            RuleFor(t => t.ReligionAndSubReligion).ChildRules(x =>
            {
                x.RuleFor(t => t.ReligionId).Required();
                x.RuleFor(t => t.SubReligionId).CustomAsync(async t =>
                {
                    if (t.SubReligionId.HasValue)
                    {
                        var flag = await provider.GetCaspianService<SubReligionService>().AnyAsync(u => u.Id == t.SubReligionId &&
                                u.ReligionId == t.ReligionId);
                        return !flag;
                    }
                    return false;
                }, "مقادیر دین و مذهب درست مقداردهی نشده اند");
            });
            RuleFor(t => t.IdentificationDetail).ChildRules(x =>
            {
                x.RuleFor(t => t.FatherName).Required();
                x.RuleFor(t => t.IdentificationNo).Required();
                x.RuleFor(t => t.BirthDate).Required();
                x.RuleFor(t => t.BirthCityId).CheckCascadeAsync(t => t.BirthProvinceId, t => t.BirthCountryId, t => t.BirthCity.Province.CountryId, "شهر، استان و کشور محل تولد درست انتخاب نشده اند");
                x.RuleFor(t => t.RegCityId).CheckCascadeAsync(t => t.RegProvinceId, t => t.RegCountryId, t => t.RegCity.CountryId, "شهر، استان و کشور محل ثبت شناسنامه درست انتخاب نشده اند");
                x.RuleFor(t => t.BirthProvinceId).CheckCascadeAsync(t => t.BirthCountryId, "استان و کشور محل تولد درست انتخاب نشده اند");
                x.RuleFor(t => t.RegProvinceId).CheckCascadeAsync(t => t.RegCountryId, "استان و کشور محل ثبت شناسنامه درست انتخاب نشده اند");
            });
            //RuleFor(t => t.IdentificationDetail.BirthCountryId.Value == )
        }
    }

    public class SubReligionService : BaseService<SubReligion>
    {
        public SubReligionService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.ReligionId, "این مورد تکراری است");
        }
    }
}
