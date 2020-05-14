namespace Infantry.LogUploader.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class ArgumentParser
    {
        public static LogUploaderArguments ParseArguments(string[] args)
        {
            var arguments = new LogUploaderArguments()
            {
                ServiceUrl = new Uri("https://joelmcbeth.com/infantry-logs/")
            };

            if (args.Length == 2 && args[0].ToLower() == "--url")
            {
                arguments.ServiceUrl = new Uri(args[1]);
            }

            return arguments;
        }
    }
}
