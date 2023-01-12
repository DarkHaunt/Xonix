using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Xonix.Saving.Core;



namespace Xonix.Saving
{
    public class JSONSaveSystem : ISaveSystem
    {
        private const string DefaultSaveFolderName = "Savings";

        private readonly string SavePath;



        public JSONSaveSystem(string folderName)
        {
            SavePath = Path.Combine(Application.persistentDataPath, DefaultSaveFolderName, folderName);
        }

        public JSONSaveSystem()
        {
            SavePath = Path.Combine(Application.persistentDataPath, DefaultSaveFolderName);
        }



        public T Load<T>(string itemName)
        {
            var fullpath = Path.Combine(SavePath, itemName);

            if (!File.Exists(fullpath))
                return default(T);

            using (StreamReader file = File.OpenText(fullpath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Auto;

                return (T)serializer.Deserialize(file, typeof(T));
            }

        }

        public void Save(object saveObject, string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();

            serializer.TypeNameHandling = TypeNameHandling.Auto;

            Directory.CreateDirectory(SavePath);
            using (StreamWriter sw = new StreamWriter(Path.Combine(SavePath, fileName)))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                serializer.Serialize(writer, saveObject);
            }
        }
    }
}
