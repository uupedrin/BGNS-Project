using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SerializationManager
{
    private const bool USE_CRYPTOGRAPHY = false;

    private static string GetSavePath(string saveName)
    {
        string saveDataDirectory = Path.Combine(Application.persistentDataPath, "saves");
        if (!Directory.Exists(saveDataDirectory))
        {
            Directory.CreateDirectory(saveDataDirectory);
        }
        return Path.Combine(saveDataDirectory, $"{saveName}.sav");
    }
    public static bool Save<T>(string saveName, T saveData)
    {
        string path = GetSavePath(saveName);

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

        if (USE_CRYPTOGRAPHY) json = EncryptionUtility.EncryptString(json);
        File.WriteAllText(path, json);

        return true;
    }

    public static T Load<T>(string saveName)
    {
        string path = GetSavePath(saveName);
        if (!File.Exists(path))
        {
            return default;
        }

        string data = File.ReadAllText(path);
        if (USE_CRYPTOGRAPHY) data = EncryptionUtility.DecryptString(data);
        T save = JsonConvert.DeserializeObject<T>(data);
        
        return save;
    }

    public static bool Exists(string saveName)
    {
        return File.Exists(GetSavePath(saveName));
    }

    public static bool Delete(string saveName)
    {
        string path = GetSavePath(saveName);
        bool exists = File.Exists(path);
        if (exists) File.Delete(path);
        return exists;
    }
}
