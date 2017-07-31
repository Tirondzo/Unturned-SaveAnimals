using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Reflection;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Unturned;

namespace SaveAnimals
{
    public class SaveAnimals : RocketPlugin<SaveAnimalsConfiguration>
    {
        internal static SaveAnimals instance;

        private object saveWatchdogInstance;
        private FieldInfo nextSaveTimeField;
        private DateTime? nextSaveTime;

        protected override void Load()
        {
            base.Load();
            SaveAnimals.instance = this;


            Level.onLevelLoaded += onLevelLoaded;
            Provider.onServerShutdown += onServerShutdown;

            if (this.Configuration.Instance.InitiateOnSaveCommand)
            {
                int index = Commander.commands.FindIndex((a) => a.command.ToLower().Equals("save"));
                if (index > -1)
                    Commander.commands[index] = new CommandSaveWrapper(Commander.commands[index]);
            }


            if (this.Configuration.Instance.InitiateOnTimeEvent)
            {
                Type rocket = Type.GetType("Rocket.Unturned.Utils.AutomaticSaveWatchdog, Rocket.Unturned");
                if (rocket != null)
                {
                    try
                    {
                        saveWatchdogInstance = rocket.GetField("Instance", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).GetValue(null);
                        nextSaveTimeField = rocket.GetField("nextSaveTime", BindingFlags.NonPublic | BindingFlags.Instance);

                        restartTimer();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(string.Format("Plugin {0} has to be updated... ", SaveAnimals.instance.Name));
                        Logger.Log(ex);
                    }
                }
                else Logger.LogError("Error in reflection to rocket");
            }
        }

        protected override void Unload()
        {
            Level.onLevelLoaded -= onLevelLoaded;
            Provider.onServerShutdown -= onServerShutdown;

            if (this.Configuration.Instance.InitiateOnSaveCommand)
            {
				int index = Commander.commands.FindIndex((a) => a.command.ToLower().Equals("save"));
				if (index > -1)
                    Commander.commands[index] = ((CommandSaveWrapper)Commander.commands[index]).commandSave;
            }
            base.Unload();
        }

        private void onLevelLoaded(int level)
        {
            AnimalsDataManager.load();
        }

        private void onServerShutdown()
        {
            AnimalsDataManager.save();
        }

        [RocketCommand("save_animals", "Save animals", "\\save_animals", AllowedCaller.Both)]
        public void ExecutSaveCommand(IRocketPlayer caller, string[] parameters)
        {
            AnimalsDataManager.save();
        }


        private void FixedUpdate()
        {
            if (this.Configuration.Instance.InitiateOnTimeEvent && U.Settings.Instance.AutomaticSave.Enabled)
            {
                if(nextSaveTime.HasValue && nextSaveTime.Value < DateTime.Now){
                    AnimalsDataManager.save();
                    restartTimer();
                }
            }
        }

        private void restartTimer(){
            if (nextSaveTimeField != null && saveWatchdogInstance != null)
                nextSaveTime = (DateTime?)nextSaveTimeField.GetValue(saveWatchdogInstance);
            else nextSaveTime = DateTime.Now.AddSeconds(U.Settings.Instance.AutomaticSave.Interval);
		}

    }
}
