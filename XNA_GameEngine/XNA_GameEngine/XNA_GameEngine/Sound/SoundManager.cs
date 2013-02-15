using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;

namespace XNA_GameEngine.Sound
{
    class SoundManager
    {
        private AudioEmitter audioEmitter;

        private static SoundManager g_SoundManager;
        
        public SoundManager()
        {
            audioEmitter = null;
        }

        public void Initialize()
        {
            audioEmitter = new AudioEmitter();
        }

        public void Update()
        {

        }

        public static SoundManager GetInstance()
        {
            if (g_SoundManager == null)
            {
                g_SoundManager = new SoundManager();
            }

            return g_SoundManager;
        }

        public void FireAndForget(string soundEffect)
        {

        }

        public void FireAndForget(string soundEffect, bool looping)
        {

        }
    }
}
