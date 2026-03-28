using Caspian.UI;
using Caspian.Common;
using System.Text.Json;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Engine.Web.Pages.MultiLanguage
{
    public partial class EntityLanguagePage: BasePage
    {
        SubsystemKind? systemKind;
        string entityName, filePath;
        Type entityType;
        IList<SelectListItem> entities;
        IList<EntityLangData> entityLangDatas;

        IList<EntityLangData> GetEntityLangDatas()
        {
            var jsonText = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<EntityLangData[]>(jsonText).Where(t => t.SubsystemKind == systemKind).ToList();
        }

        void SaveData()
        {
            bool titleIsEmpty = false;
            foreach (var item in entityLangDatas)
                if (!item.Title.HasValue())
                    titleIsEmpty = true;
            if (!titleIsEmpty)
            {
                var jsonText = File.ReadAllText(filePath);
                var others = JsonSerializer.Deserialize<EntityLangData[]>(jsonText).Where(t => t.SubsystemKind != systemKind || t.TypeName != entityName).ToList();
                others.AddRange(entityLangDatas);
                jsonText = JsonSerializer.Serialize(others);
                File.WriteAllText(filePath, jsonText);
                ShowMessage("ثبت با موفقیت انجام شد");
            }
            else
                ShowMessage("Please spisify all properties title");
        }

        protected override void OnInitialized()
        {
            filePath = $"{Host.ContentRootPath}/Languages/{CultureInfo.CurrentCulture.Name}.json";
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, "[]");
            base.OnInitialized();
        }

        void FillEntities()
        {
            if (systemKind.HasValue)
            {
                entities = systemKind.Value.GetEntityAssembly().GetTypes().Where(t => t.GetCustomAttribute<TableAttribute>() != null).Select(t => new SelectListItem()
                {
                    Value = t.Name,
                    Text = t.GetCustomAttribute<ForeignKeyAttribute>()?.Name ?? t.Name
                }).ToList();
            }
        }

        void FillLangData()
        {
            var jsonText = File.ReadAllText(filePath);
            entityType = systemKind.Value.GetEntityAssembly().GetTypes().Single(t => t.Name == entityName);
            entityLangDatas = JsonSerializer.Deserialize<EntityLangData[]>(jsonText).Where(t => t.SubsystemKind == systemKind && t.TypeName == entityName).ToList();
            foreach (var info in entityType.GetProperties())
            {
                var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                if (attr != null)
                {
                    var old = entityLangDatas.SingleOrDefault(t => t.MemberName == info.Name);
                    if (old == null)
                    {
                        old = new EntityLangData()
                        {
                            SubsystemKind = systemKind.Value,
                            TypeName = entityName,
                            MemberName = info.Name
                        };
                        entityLangDatas.Add(old);
                    }
                }
            }
        }
    }
}
