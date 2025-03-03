using Caspian.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Reflection;

namespace Caspian.UI
{
    public static class Extension
    {
        public static string GetText(this IList<SelectListItem> items, string value) 
        {
            var item = items.SingleOrDefault(t => t.Value == value);
            return item?.Text;
        }

        internal static void SetUserId(this IServiceScope scope, int userId)
        {
            scope.GetService<CaspianDataService>().UserId = userId;
        }

        internal static void SetUserId(this IServiceScope scope, PageData pageData)
        {
            if (pageData != null)
            {
                var service = scope.GetService<CaspianDataService>();
                service.UserId = pageData.UserId;
            }
        }

        public static string GetCssClassName(this DefaultLayout layout)
        {
            switch (layout)
            {
                case DefaultLayout.SpaceBetween: return "between";
                case DefaultLayout.SpaceEvenly: return "evenly";
                case DefaultLayout.FlexStart: return "start";
                case DefaultLayout.FlexEnd: return "end";
                case DefaultLayout.SpaceAround: return "around";
                case DefaultLayout.Center: return "center";
            }
            throw new NotImplementedException();
        }

        public async static Task<byte[]> GetByteArrayAsync(this IBrowserFile file)
        {
            using var stream = file.OpenReadStream(51200000);
            var buffer = new byte[stream.Length];
            var sum = 0;
            var remine = (int)stream.Length - sum;
            var count = await stream.ReadAsync(buffer, 0, remine);
            while (count > 0)
            {
                sum += count;
                remine = (int)stream.Length - sum;
                if (remine > 10240)
                    remine = 10240;
                count = await stream.ReadAsync(buffer, sum, remine);
            }
            return buffer;
        }

        internal static void RemoveFieldState(this EditContext editContext, string fieldName)
        {
            var field = editContext.GetType().GetField("_fieldStates", BindingFlags.NonPublic | BindingFlags.Instance);
            var states = (field.GetValue(editContext) as System.Collections.IDictionary);
            DictionaryEntry? old = null;
            foreach (DictionaryEntry state in states)
            {
                var fieldName1 = (state.Key as dynamic).FieldName as string;
                if (fieldName1 == fieldName)
                {
                    old = state;
                    break;
                }
            }
            if (old.HasValue)
                states.Remove(old.Value.Key);
        }
    }
}
