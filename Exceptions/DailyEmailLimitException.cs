namespace FiresportCalendar.Exceptions
{
    public class DailyEmailLimitException : Exception
    {
        public DailyEmailLimitException()
                   : base("Denní limit odesílání emailů byl dosažen.\n V případě nutnosti se prosím obraťte na admina.")
        {
        }
    }
}
