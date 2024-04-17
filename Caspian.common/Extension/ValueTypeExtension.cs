using System.Reflection;
using System.Globalization;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Common
{
    public static class ValueTypeExtension
    {
        public static string Seprate3Digit(this int value)
        {
            return string.Format("{0:#,0}", value);
        }

        public static string Seprate3Digit(this long value)
        {
            return string.Format("{0:#,0}", value);
        }

        public static string Seprate3Digit(this int? value)
        {
            if (value.HasValue)
                return string.Format("{0:#,0}", value);
            return "";
        }

        public static string ToStringValue(this int? value)
        {
            if (value.HasValue)
                return value.ToString();
            return "";
        }

        public static string ConvertToBrowserDate(this DateTime? date)
        {
            if (date == null)
                return "";
            return date.ConvertToBrowserDate();
        }

        public static string ConvertToBrowserDate(this DateTime date)
        {
            var str = date.Year.ToString() + '-';
            if (date.Month < 10)
                str += '0';
            str += date.Month.ToString() + '-';
            if (date.Day < 10)
                str += "0";
            str += date.Day.ToString();
            return str;
        }

        public static string Seprate3Digit(this long? value)
        {
            if (value.HasValue)
                return string.Format("{0:#,0}", value);
            return "";
        }

        public static string LongString(this TimeSpan time)
        {
            var str = "";
            if (time.Hours < 10)
                str += "0";
            str += time.Hours.ToString() + ":";
            if (time.Minutes < 10)
                str += "0";
            str += time.Minutes;
            if (time.Seconds < 10)
                str += "0";
            str += time.Seconds;
            return str;
        }

        public static string ToJsonDecimal(this decimal value)
        {
            return value.ToString().Replace('/', '.');
        }

        public static string ShortString(this TimeSpan time)
        {
            var str = "";
            if (time.Hours < 10)
                str += "0";
            str += time.Hours.ToString() + ":";
            if (time.Minutes < 10)
                str += "0";
            str += time.Minutes;
            return str;
        }

        public static string Seprate3Digit(this decimal value)
        {
            if (value == 0)
                return "0";
            var str = String.Format("{0:#,#.###}", value);
            str = str.Replace('/', '.');
            if (str.StartsWith("."))
                return '0' + str;
            return str;
        }

        public static DateTime FirstDayInCalendar(this DateTime  date)
        {
            var day = date.Day;
            date = date.AddDays(-day);
            day = date.DayOfWeek.ConvertToInt().Value;
            return date.AddDays(-day);
        }

        public static PersianDate FirstDayInCalendar(this PersianDate date)
        {
            var day = date.Day.Value;
            date = date.AddDays(-day + 1);
            day = date.DayOfWeek.ConvertToInt().Value - 1;
            return date.AddDays(-day);
        }

        public static string Seprate3Digit(this decimal? value)
        {
            if (value.HasValue)
                return value.Value.Seprate3Digit();
            return "";
        }

        public static PersianDate ToPersianDate(this DateTime date)
        {
            var calendar = new PersianCalendar();
            return new PersianDate(calendar.GetYear(date), (PersianMonth)calendar.GetMonth(date), calendar.GetDayOfMonth(date), date.Hour, 
                date.Minute, date.Second);
        }

        public static PersianDate ToPersianDate(this DateTime? date)
        {
            if (date == null)
                return null;
            return date.Value.ToPersianDate();
        }

        public static string ToPersianDateString(this DateTime date)
        {
            return date.ToPersianDate().ToString();
        }

        public static DateTime ChangeDay(this DateTime date, int day)
        {
            return new DateTime(date.Year, date.Month, day);
        }

        public static double Floor(this double d, int digits)
        {
            d = d * Math.Pow(10, digits);
            d = Convert.ToInt32(d);
            return d / Math.Pow(10, digits);
        }

        //public static int? ConvertToInt(this Enum curentEnum)
        //{
        //    if (curentEnum == null)
        //        return null;
        //    return Convert.ToInt32(curentEnum);
        //}

        public static string ToPersianDateStringDayOfWeek(this DateTime date)
        {
            var pdate = date.ToPersianDate();
            return pdate.DayOfWeek.EnumText() + " " + pdate.ToString();
        }

        public static string ToPersianDateString(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToPersianDateString();
            return "";
        }

        public static string ToShortDateString(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToShortDateString();
            return "";
        }

        public static Dictionary<TValue, string> GetEnumFields<TValue>(this TValue currentEnum)where TValue: Enum
        {
            var enumes = new Dictionary<TValue, string>();
            foreach (FieldInfo fi in currentEnum.GetType().GetFields().Where(t => !t.IsSpecialName))
            {
                var da = fi.GetCustomAttribute<DisplayAttribute>();
                if (da != null)
                    enumes.Add((TValue)fi.GetValue(currentEnum), da.Name);
                else
                    enumes.Add((TValue)fi.GetValue(currentEnum), fi.Name);
            }
            return enumes;
        }

        public static string CallFormat(this string number, char chr)
        {
            if (number.HasValue() && number.Length == 11)
                return number.Substring(0, 4) + chr + number.Substring(4, 3) + chr + number.Substring(7, 4);
            return number;
        }

        public static string EnumText(this Enum field, string nullText)
        {
            if (nullText != null)
            {
                if (field == null || field.ToString() == "0")
                    return nullText;
            }
            return field.EnumText();
        }

        public static Dictionary<string, string> DisplayNames(this Enum currentEnum)
        {
            Dictionary<string, string> enumes = new Dictionary<string, string>();
            foreach (FieldInfo fi in currentEnum.GetType().GetFields().Where(t => !t.IsSpecialName))
            {
                var da = fi.GetCustomAttribute<DisplayAttribute>();
                if (da != null)
                    enumes.Add(da.Name, fi.GetValue(currentEnum).ToString());
                else
                    enumes.Add(fi.Name, Convert.ToInt32(fi.GetValue(currentEnum)).ToString());
            }
            return enumes;
        }
    }
}
