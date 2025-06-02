using System;
using System.Linq;

namespace Playbox.CI
{
    public static class SmartEnviroment
    {
        public static string GetArgumentValue(string argumentName)
        {
            var args = Environment.GetCommandLineArgs().ToList();
            var argIndex = args.FindIndex(x => x == argumentName);
            
            var value = args[argIndex + 1];

            if (argIndex != -1 && argIndex < args.Count)
            {
                return args[argIndex];
            }
            else
            {
                return null;
            }
        }

        public static int GetArgumentIntValue(string argumentName, int defaultValue = 0)
        {
            return int.TryParse(GetArgumentValue(argumentName), out var result) ? result : defaultValue;
        }
        
        public static float GetArgumentFloatValue(string argumentName, float defaultValue = 0)
        {
            return float.TryParse(GetArgumentValue(argumentName), out var result) ? result : defaultValue;
        }

        public static bool HasArgument(string argumentName)
        {
            if (string.IsNullOrEmpty(argumentName))
                return false;
            
            return Environment.GetCommandLineArgs().Any(arg => arg == argumentName);
        }
    }
}
