namespace Engine
{
    public class Debug
    {
        #region Settings

        public const string c_Directory = @"Logs\";
        public const string c_Extension = ".log";

        #endregion


        internal static Debug? active;

        private StreamWriter Writer;

        public Debug()
        {
            active = this;

            string _directory = Path.Combine(Environment.CurrentDirectory, c_Directory);
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            string _file = Path.Combine(_directory, DateTime.Now.ToFileTimeUtc() + c_Extension);
            Writer = new StreamWriter(_file, true);
            Writer.AutoFlush = true;

            Log($"The logger has been successfully initialized. Output file: {_file}", this);
        }

        public static void Log(object? obj, object? context = null)
        {
            string msg = "[Log] " + (context != null ? $"[{context.GetType().Name}] {obj}" : obj?.ToString());
            Console.WriteLine(msg);
            active?.Writer.WriteLine(msg);
        }
        public static void LogError(object? obj, object? context = null)
        {
            string msg = "[Error] " + (context != null ? $"[{context.GetType().Name}] {obj}" : obj?.ToString());
            Console.WriteLine(msg);
            active?.Writer.WriteLine(msg);
        }
        public static void LogException(Exception exception, object? context = null)
        {
            string msg = "[Exception] " + (context != null ? $"[{context.GetType().Name}] {exception.Message}" : exception.Message);
            Console.WriteLine(msg);
            active?.Writer.WriteLine(msg);
        }
    }
}
