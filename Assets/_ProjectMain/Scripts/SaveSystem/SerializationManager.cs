using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SerializationManager
{
    private const bool USE_CRYPTOGRAPHY = false;
    public static bool Save<T>(string saveName, T saveData)
    {
        string saveDataDirectory = Path.Combine(Application.persistentDataPath, "saves");

        if(!Directory.Exists(saveDataDirectory))
        {
            Directory.CreateDirectory(saveDataDirectory);
        }

        string path = Path.Combine(saveDataDirectory, $"{saveName}.sav");

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

        if (USE_CRYPTOGRAPHY) json = EncryptionUtility.EncryptString(json);
        File.WriteAllText(path, json);

        return true;
    }

    public static T Load<T>(string saveName, string path = null)
    {
        path ??= Path.Combine(Application.persistentDataPath, "saves");
        path = Path.Combine(path, $"{saveName}.sav");
        if (!File.Exists(path))
        {
            return default;
        }

        string data = File.ReadAllText(path);
        if (USE_CRYPTOGRAPHY) data = EncryptionUtility.DecryptString(data);
        T save = JsonConvert.DeserializeObject<T>(data);
        
        return save;
    }
}
