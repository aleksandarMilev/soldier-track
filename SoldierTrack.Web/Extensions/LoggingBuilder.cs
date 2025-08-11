namespace SoldierTrack.Web.Extensions
{
    public static class LoggingBuilder
    {
        public static void AddLogging(this ILoggingBuilder builder)
        { 
            builder.ClearProviders();
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        }
    }  
}