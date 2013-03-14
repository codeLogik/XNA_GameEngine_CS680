using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Debug
{
    class DebugTools
    {
        static public void Report(String log)
        {
            System.Diagnostics.Debug.WriteLine(log);
        }

        static public void Log(String system, String activity, String message)
        {
            Console.WriteLine("[{0}] ({1}): {2}", system, activity, message);
        }


    }
}
