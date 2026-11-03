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
            RuleFor(t => t.IdCard).Required().UniqueAsync("A user with this National ID already exists in the system.").CheckIdCard();
            RuleFor(t => t.EmploymentNo).Required().UniqueAsync("An employee with this code is already registered in the system.");
            RuleFor(t => t.ScopeId).CustomAsync(async t => 
            {
                var result = await provider.GetCaspianService<SimpleDataService>().GetAll()
                    .AnyAsync(u => u.DataType == SimpleDataType.Scope && t.ScopeId == u.Id);
                return !result;
            }, "No Department with this code has been defined in the system.");
            RuleFor(t => t.CostCenterId).CustomAsync(async t =>
            {
                if (t.CostCenterId == null)
                    return false;
                var result = await provider.GetCaspianService<SimpleDataService>().GetAll()
                    .AnyAsync(u => u.DataType == SimpleDataType.CostCenter && t.CostCenterId == u.Id);
                return !result;
            }, "No cost center with this code has been defined in the system.");
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
                }, "The religion and denomination values have not been set correctly.");
            });
            RuleFor(t => t.IdentificationDetail).ChildRules(x =>
            {
                x.RuleFor(t => t.FatherName).Required();
                x.RuleFor(t => t.IdentificationNo).Required();
                x.RuleFor(t => t.BirthDate).Required();
                x.RuleFor(t => t.BirthCityId).CheckCascadeAsync(t => t.BirthProvinceId, t => t.BirthCountryId, t => t.BirthCity.Province.CountryId, "The birth city, province, and country have not been selected correctly.");
                x.RuleFor(t => t.RegCityId).CheckCascadeAsync(t => t.RegProvinceId, t => t.RegCountryId, t => t.RegCity.CountryId, "The city, province, and country of birth certificate registration have not been selected correctly.");
                x.RuleFor(t => t.BirthProvinceId).CheckCascadeAsync(t => t.BirthCountryId, "The birth province and country have not been selected correctly.");
                x.RuleFor(t => t.RegProvinceId).CheckCascadeAsync(t => t.RegCountryId, "The province and country of birth certificate registration have not been selected correctly.");
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
