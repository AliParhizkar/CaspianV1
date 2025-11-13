using Syncfusion.Blazor;
using Syncfusion.Blazor.Inputs;
using System.Text.RegularExpressions;

namespace Main
{
    /// <summary>
    /// Extends ISyncfusionStringLocalizer for applying localization to Syncfusion components.
    /// </summary>
    public class SyncfusionLocalizer : ISyncfusionStringLocalizer
    {
        /// <summary>
        /// Get locale value from the resource file.
        /// </summary>
        /// <param name="key">Locale key for getting the translated text.</param>
        public string GetText(string key)
        {
            var index = key.IndexOf('_') + 1;
            key = key.Substring(index);
            return  Regex.Replace(key, "(\\B[A-Z])", " $1");
        }

        /// <summary>
        /// Access the resource file and get the exact value from the locale key.
        /// </summary>
        public System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return null;
            }
        }
    }
}
