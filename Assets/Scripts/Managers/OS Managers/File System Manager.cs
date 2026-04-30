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
    public FolderNode DesktopFolder { get; private set; }

    public bool DeleteSaveAtStart = false;

    void Start()
    {
        if (DeleteSaveAtStart) this.DeleteSave("filesystem");

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

        CreateFile(DesktopFolder, "Readme.txt", FileType.Text, "Welcome to the OS Simulation! This is a sample text file on your desktop. Feel free to open it and explore the contents. You can create new folders and files, and even customize your desktop with images! Enjoy your experience simulating an operating system environment.");
        CreateFile(DesktopFolder, "Picture.png", FileType.Image, "Icon_Folder");

        this.Save("filesystem");
    }

    public FolderNode CreateFolder(FolderNode parent, string name)
    {
        FolderNode newFolder = new FolderNode(name, parent);
        parent.AddChild(newFolder);
        this.Save("filesystem");
        return newFolder;
    }

    public FileNode CreateFile(FolderNode parent, string name, FileType type, string Content)
    {
        FileNode newFile = new FileNode(name, type, parent) { Content = Content };
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