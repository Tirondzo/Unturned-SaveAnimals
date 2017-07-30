using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveAnimals
{
    public class SaveAnimalsConfiguration : IRocketPluginConfiguration
    {
        public bool InitiateOnSaveCommand = true;
        public bool InitiateOnTimeEvent = true;

        public void LoadDefaults()
        {
            InitiateOnSaveCommand = true;
            InitiateOnTimeEvent = true;
        }
    }
}
