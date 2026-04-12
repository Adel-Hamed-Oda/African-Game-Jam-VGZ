using System.IO;
using UnityEngine;

public interface ISerializable<TData> where TData : class, new()
{
    TData SaveData { get; set; }
}

public static class SerializableExtensions
{
    private static string SavePath => Application.persistentDataPath + "/saves/";
    private static string FullPath(string key) => SavePath + key + ".json";

    public static void Save<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        string json = JsonUtility.ToJson(target.SaveData, prettyPrint: true);

        Directory.CreateDirectory(SavePath);
        File.WriteAllText(FullPath(key), json);

        Debug.Log($"Saved {key} to {FullPath(key)}");
    }

    public static void Load<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        string path = FullPath(key);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save file found at {path}");
            return;
        }

        target.SaveData = JsonUtility.FromJson<TData>(File.ReadAllText(path));

        Debug.Log($"Loaded {key} from {path}");
    }
}