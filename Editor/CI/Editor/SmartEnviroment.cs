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
            
            return value;
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
            return Environment.GetCommandLineArgs().Any(arg => arg == argumentName);
        }
    }
}
