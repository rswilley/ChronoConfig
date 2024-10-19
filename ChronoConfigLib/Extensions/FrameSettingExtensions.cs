using System.Text;

namespace ChronoConfigLib.Extensions
{
    public static class FrameSettingListExtensions
    {
        public static string ToSchedule(this IEnumerable<FrameSetting> frameSettings)
        {
            var sb = new StringBuilder();
            foreach (var item in frameSettings)
            {
                if (item.UseFormula)
                {
                    sb.Append($"{item.FrameNumber}: ({item.Formula}), ");
                }
                else
                {
                    sb.Append($"{item.FrameNumber}: ({item.FrameValue}), ");
                }
            }
            
            var result = sb.ToString().Trim();
            result = result.Substring(0, result.Length - 1);

            return result;
        }
    }
}
