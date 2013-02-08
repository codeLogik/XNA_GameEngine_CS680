using System;

namespace XNA_GameEngine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CoreMain game = new CoreMain())
            {
                game.Run();
            }
        }
    }
#endif
}

