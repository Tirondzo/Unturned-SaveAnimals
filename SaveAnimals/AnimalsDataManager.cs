using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDG.Unturned;
using System.Reflection;
using Rocket.Core;
using Rocket.Core.Logging;

namespace SaveAnimals
{
    public static class AnimalsDataManager
    {
        public static readonly byte SAVEDATA_VERSION = (byte)1;

        public static void save()
        {
            River river = LevelSavedata.openRiver("/Animals.dat", false);
            river.writeByte(AnimalsDataManager.SAVEDATA_VERSION);

            river.writeUInt16((ushort)AnimalManager.packs.Count);

            foreach(PackInfo pack in AnimalManager.packs)
            {
                List<Animal> aliveAnimals = pack.animals.Where((a) => !a.isDead).ToList();
                  
                river.writeUInt16((ushort)aliveAnimals.Count);
                foreach(Animal animal in aliveAnimals)
                {
                    river.writeUInt16(animal.id);
                    river.writeSingleVector3(animal.transform.position);
                    river.writeSingle(animal.transform.rotation.eulerAngles.y);
                }
            }
            river.closeRiver();
            Logger.Log(string.Format("Saved {0} packs with {1} animals in total", AnimalManager.packs.Count, AnimalManager.animals.Count));
        }

        public static void load()
        {
            if (LevelSavedata.fileExists("/Animals.dat"))
            {
                River river = LevelSavedata.openRiver("/Animals.dat", true);
                byte ver = river.readByte();

                ushort packsCount = river.readUInt16();
                while (packsCount > AnimalManager.packs.Count) AnimalManager.packs.Add(new PackInfo());

                int loadedAnimals = 0;

                for(int i = 0; i < packsCount; i++)
                {
                    ushort animalsCount = river.readUInt16();
                    for (int j = 0; j < animalsCount; j++) {
                        
                        ushort id = river.readUInt16();
                        UnityEngine.Vector3 position = river.readSingleVector3();
                        float angle = river.readSingle();

                        if (animalsCount > AnimalManager.packs[i].animals.Count)
                        {
                            try
                            {
                                AnimalManager aManager = ((AnimalManager)(typeof(AnimalManager)).GetField("manager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(null));
                                Animal animal = (Animal)typeof(AnimalManager).GetMethod("addAnimal", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(aManager, new object[] { id, position, angle, false });
                                animal.pack = AnimalManager.packs[i];
                                AnimalManager.packs[i].animals.Add(animal);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(string.Format("Plugin {0} has to be updated... ", SaveAnimals.instance.Name));
                                Logger.Log(ex);
                                return;
                            }
                        }
                        else
                        {
                            Animal animal = AnimalManager.packs[i].animals[j];
                            animal.id = id;
                            animal.transform.position = position;
                            animal.transform.rotation = UnityEngine.Quaternion.Euler(0.0f, angle, 0.0f);
                        }
                    }

                    loadedAnimals += animalsCount;
                }

                river.closeRiver();
                Logger.Log(string.Format("Loaded {0} packs with {1} animals in total", packsCount, loadedAnimals));
            }
		}

    }
}
