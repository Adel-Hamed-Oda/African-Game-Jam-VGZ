using Newtonsoft.Json;
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

    // The magic settings that give you full freedom
    private static JsonSerializerSettings GetSettings()
    {
        return new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto, // Fixes Polymorphism (FileNode vs FolderNode)
            PreserveReferencesHandling = PreserveReferencesHandling.Objects // Fixes Parent/Child recursive loops!
        };
    }

    public static void Save<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        string json = JsonConvert.SerializeObject(target.SaveData, GetSettings());

        Directory.CreateDirectory(SavePath);
        File.WriteAllText(FullPath(key), json);
    }

    public static bool Load<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        string path = FullPath(key);

        if (!File.Exists(path)) return false;

        target.SaveData = JsonConvert.DeserializeObject<TData>(File.ReadAllText(path), GetSettings());
        return true;
    }

    public static bool SaveExists<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        return File.Exists(FullPath(key));
    }

    public static void DeleteSave<TData>(this ISerializable<TData> target, string key) where TData : class, new()
    {
        string path = FullPath(key);
        if (File.Exists(path)) File.Delete(path);
    }
}