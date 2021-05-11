using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FireworksMania.Tools.BlueprintTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("########### DISCLAIMER ###########");
            Console.WriteLine("MAKE SURE YOU HAVE A BACKUP OF YOUR BLUEPRINT - AS THIS TOOL WAS MADE DURING A SNEEZE!");
            Console.WriteLine("########### ********** ###########");

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("########### INSTRUCTIONS ###########");
            Console.WriteLine("This tool will move all your fireworks in the blueprint up a bit, so old Ranch blueprints still works after the Ranch have been moved up a bit");
            Console.WriteLine("########### ************ ###########");

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("## Copy the blueprint file(s) you want to fix to the same location as this application! ##");

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Write the blueprint name, including .json - ex. My fancy show.json");

            var fileName = Console.ReadLine();

            if (String.IsNullOrEmpty(fileName))
                Console.WriteLine("Ops, not a valid filename... I quit!");

            var blueprintData = JsonConvert.DeserializeObject<SaveableBlueprintData>(File.ReadAllText(fileName));

            foreach (var entity in blueprintData.Entities)
            {
                foreach (var key in entity.CustomComponentData.Keys)
                {
                    var componentData = entity.CustomComponentData[key];
                    var position = componentData.Get<SerializableVector3>("Position");
                    position.Y += 7f;
                    componentData.CustomData["Position"] = position;
                }
            }

            var newBlueprintJson = JsonConvert.SerializeObject(blueprintData);
            var fixedBlueprintFileName = $"Fixed_{fileName}";
            
            if (File.Exists(fixedBlueprintFileName))
                File.Delete(fixedBlueprintFileName);

            File.WriteAllText(fixedBlueprintFileName, newBlueprintJson);

            Console.WriteLine($"Blueprint has been fixed and saved as '{fixedBlueprintFileName}'");
            
            Console.ReadLine();
        }


        [Serializable]
        public class SaveableBlueprintMetaData
        {
            public string Author;
            public string GameVersion;
            public string Map;
            public string Description;
            public DateTime ModifiedUtc;
        }


        [Serializable]
        public class SaveableBlueprintData : SaveableBlueprintMetaData
        {
            [JsonProperty(Order = 100)]
            public IEnumerable<SaveableEntityData> Entities;
        }

        [Serializable]
        public struct SaveableEntityData
        {
            public string EntityInstanceId;
            public string EntityDefinitionId;
            public Dictionary<string, CustomEntityComponentData> CustomComponentData;
        }

        [Serializable]
        public struct CustomEntityComponentData
        {
            public Dictionary<string, object> CustomData;

            private void InitializeCustomDataIfNotAlready()
            {
                if (CustomData == null)
                    CustomData = new Dictionary<string, object>();
            }

            public void Add<T>(string key, T data)
            {
                InitializeCustomDataIfNotAlready();

                CustomData.Add(key, data);
            }

            public T Get<T>(string key)
            {
                InitializeCustomDataIfNotAlready();

                object foundData = null;

                if (CustomData.TryGetValue(key, out foundData))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(foundData.ToString());
                    }
                    catch
                    {
                        //Note: Never do a catch like this kids - unless you are really sure you don't want to know if this fails!
                    }

                    try
                    {
                        return (T)foundData;
                    }
                    catch
                    {
                        //Note: Never do a catch like this kids - unless you are really sure you don't want to know if this fails!
                    }
                }

                return default(T);
            }
        }

        [Serializable]
        public struct SerializableVector3
        {
            public float X;
            public float Y;
            public float Z;
        }

        [Serializable]
        public struct SerializableRotation
        {
            public float X;
            public float Y;
            public float Z;
            public float W;
        }
    }
}
