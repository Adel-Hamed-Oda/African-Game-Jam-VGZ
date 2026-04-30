using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : SingletonBehaviour<ImageManager>
{
    // --- Inspector Structs ---
    [Serializable] public struct FileTypeIcon
    {
        public string imageName;
        public FileType type;
    }

    [Serializable] public struct NamedImage
    {
        public string imageName;
        public Sprite sprite;
    }

    [Header("Folder & Default Icons")]
    public string folderImageName = "Icon_Folder";
    public string unknownImageName = "Icon_Unknown";

    [Header("File Type Mappings")]
    public List<FileTypeIcon> inspectorFileTypeIcons = new List<FileTypeIcon>();

    [Header("All Images Database")]
    public List<NamedImage> inspectorAllImages = new List<NamedImage>();

    // --- The Runtime Dictionaries ---
    private Dictionary<FileType, string> fileTypeToImageName = new Dictionary<FileType, string>();
    private Dictionary<string, Sprite> nameToImage = new Dictionary<string, Sprite>();

    protected override void Awake()
    {
        base.Awake();

        foreach (var item in inspectorFileTypeIcons)
        {
            fileTypeToImageName[item.type] = item.imageName;
        }

        foreach (var item in inspectorAllImages)
        {
            nameToImage[item.imageName] = item.sprite;
        }
    }

    // --- Core Functions ---

    public Sprite GetImageByName(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) return null;

        if (nameToImage.TryGetValue(imageName, out Sprite foundSprite))
        {
            return foundSprite;
        }

        // Fallback if the image name isn't found
        Debug.LogWarning($"Image '{imageName}' not found in ImageManager!");
        return nameToImage.ContainsKey(unknownImageName) ? nameToImage[unknownImageName] : null;
    }

    public Sprite GetImageForNode(FSNode node)
    {
        if (node is FolderNode)
        {
            return GetImageByName(folderImageName);
        }
        else if (node is FileNode fileNode)
        {
            return GetImageByName(GetImageNameForFileType(fileNode.Type));
        }

        return GetImageByName(unknownImageName);
    }

    private string GetImageNameForFileType(FileType type)
    {
        if (fileTypeToImageName.TryGetValue(type, out string imageName))
        {
            return imageName;
        }
        return unknownImageName; // Fallback
    }
}