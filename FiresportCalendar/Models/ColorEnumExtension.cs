namespace FiresportCalendar.Models
{
    public static class ColorEnumExtension
    {
        public static string ToCssClass(this ColorEnum? color)
        {
            if (!color.HasValue || !Enum.IsDefined(typeof(ColorEnum), color))
                return "bg-pink";

            return $"bg-{color.Value.ToString().ToLower()}";
        }
    }
}
