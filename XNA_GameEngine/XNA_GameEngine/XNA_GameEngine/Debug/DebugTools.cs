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
    }
}
