using Demo.Model;
using Demo.Service;
using Caspian.Common;
using Caspian.Common.Service;
using System.ComponentModel.DataAnnotations;

namespace Main.Model
{
    public class TestModel
    {
        [Key]
        public int Id { get; set; }

        public Country Country { get; set; }

        public City City { get; set; }

        public string Name { get; set; }

    }

    public class TestModelService : BaseService<TestModel>
    {
        public TestModelService(IServiceProvider provider):
            base(provider)
        {
            RuleFor(t => t.Country).SetValidator(new CountryService(provider));
            RuleFor(t => t.City).SetValidator(new CityService(provider));
            RuleFor(t => t.Name).Required();
        }
    }
}
