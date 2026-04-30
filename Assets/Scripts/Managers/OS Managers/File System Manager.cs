using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileSystemManager : SingletonBehaviour<FileSystemManager>, ISerializable<FileSystemManager.FSData>
{
    [Serializable]
    public class FSData
    {
        public FolderNode Root; // You can just save the Root directly now!
    }

    public FSData SaveData { get; set; } = new();

    public Sprite defaultImage; // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAERLVBAE

    public FolderNode DesktopFolder { get; private set; }

    void Start()
    {
        if (this.SaveExists("filesystem"))
        {
            this.Load("filesystem");
            DesktopFolder = GetNodeByPath("C:/Desktop") as FolderNode;
        }
        else
        {
            CreateNewInstance();
        }

        if (Desktop.Instance != null && DesktopFolder != null)
        {
            Desktop.Instance.Initialize(DesktopFolder);
        }
    }

    private void CreateNewInstance()
    {
        SaveData.Root = new FolderNode("C:", null);
        DesktopFolder = CreateFolder(SaveData.Root, "Desktop");

        CreateFile(DesktopFolder,"Bruh", defaultImage);
        CreateFile(DesktopFolder, "Readme.txt", "Welcome to the OS Simulation! This is a sample text file on your desktop. Feel free to open it and explore the contents. You can create new folders and files, and even customize your desktop with images! Enjoy your experience simulating an operating system environment.");

        this.Save("filesystem");
    }

    public FolderNode CreateFolder(FolderNode parent, string name)
    {
        FolderNode newFolder = new FolderNode(name, parent);
        parent.AddChild(newFolder);
        this.Save("filesystem");
        return newFolder;
    }

    public FileNode CreateFile(FolderNode parent, string name, FileType type)
    {
        FileNode newFile = new FileNode(name, type, parent);
        parent.AddChild(newFile);
        this.Save("filesystem");
        return newFile;
    }
    public FileNode CreateFile(FolderNode parent, string name, string content)
    {
        FileNode newFile = new FileNode(name, FileType.Text, parent) { Content = content };
        parent.AddChild(newFile);
        this.Save("filesystem");
        return newFile;
    }
    public FileNode CreateFile(FolderNode parent, string name, Sprite Image)
    {
        FileNode newFile = new FileNode(name, FileType.Image, parent) { Image = Image };
        parent.AddChild(newFile);
        this.Save("filesystem");
        return newFile;
    }

    public void UpdateContent(FileNode node, string newContent)
    {
        node.Content = newContent;
        this.Save("filesystem");
    }

    public FSNode GetNodeByPath(string path)
    {
        string[] parts = path.Split('/');
        if (parts[0] != SaveData.Root.Name) return null;

        FSNode currentNode = SaveData.Root;
        for (int i = 1; i < parts.Length; i++)
        {
            if (currentNode is FolderNode folder)
            {
                currentNode = folder.Children.FirstOrDefault(c => c.Name == parts[i]);
                if (currentNode == null) return null;
            }
            else return null;
        }
        return currentNode;
    }
}
public abstract class FSNode
{
    public string Name { get; set; }
    public FolderNode Parent { get; set; }

    public string GetPath()
    {
        if (Parent == null) return Name; // Root node (e.g., "C:")
        return Parent.GetPath() + "/" + Name;
    }
}
public enum FileType
{
    Empty,
    Text,
    Image, // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAERLVBAE
    Executable
}
public class FileNode : FSNode
{
    public FileType Type { get; set; }
    public string Content { get; set; }

    [JsonIgnore]
    public Sprite Image { get; set; } // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAERLVBAE
    public FileNode(string name, FileType type, FolderNode parent)
    {
        Name = name;
        Parent = parent;
        Type = type;
    }
}
public class FolderNode : FSNode
{
    public List<FSNode> Children { get; private set; } = new List<FSNode>();

    public FolderNode(string name, FolderNode parent)
    {
        Name = name;
        Parent = parent;
    }

    public void AddChild(FSNode node)
    {
        if (!Children.Contains(node))
        {
            node.Parent = this;
            Children.Add(node);
        }
    }

    public void RemoveChild(FSNode node)
    {
        if (Children.Contains(node))
        {
            node.Parent = null;
            Children.Remove(node);
        }
    }
}