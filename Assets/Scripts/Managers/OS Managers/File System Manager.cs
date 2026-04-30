using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 1. Flattened Data Structure (JsonUtility safe!)
[Serializable]
public class FlatNodeData
{
    public string Name;
    public bool IsFolder;
    public FileType Type;
    public string Content;
    public int ParentIndex; // Stores the index of the parent folder instead of a recursive object
}

public class FileSystemManager : SingletonBehaviour<FileSystemManager>, ISerializable<FileSystemManager.FSData>
{
    [Serializable]
    public class FSData
    {
        // JsonUtility CAN serialize a simple list of flat objects!
        public List<FlatNodeData> Nodes = new List<FlatNodeData>();
    }

    public FSData SaveData { get; set; } = new();

    public FolderNode Root { get; private set; }
    public FolderNode DesktopFolder { get; private set; }

    public Sprite defaultImage; // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAE

    void Start()
    {
        this.DeleteSave("filesystem"); // Uncomment this line to reset your save data while testing!

        if (this.SaveExists("filesystem"))
        {
            LoadSystem();
            Debug.Log("System Loaded from Save!");
        }
        else
        {
            CreateNewInstance();
            Debug.Log("Created New System!");
        }

        // Make sure you use DesktopUI if that's the name of your script!
        if (Desktop.Instance != null && DesktopFolder != null)
        {
            Desktop.Instance.Initialize(DesktopFolder);
        }
    }

    private void SaveSystem()
    {
        // Clear the old save data
        SaveData.Nodes.Clear();

        // Start flattening from the root (Root has no parent, so index is -1)
        if (Root != null)
        {
            FlattenNode(Root, -1);
        }

        this.Save("filesystem");
    }

    private void FlattenNode(FSNode liveNode, int parentIndex)
    {
        int currentIndex = SaveData.Nodes.Count;

        FlatNodeData data = new FlatNodeData
        {
            Name = liveNode.Name,
            ParentIndex = parentIndex
        };

        if (liveNode is FolderNode folder)
        {
            data.IsFolder = true;
            SaveData.Nodes.Add(data); // Add self to list BEFORE processing children

            foreach (FSNode child in folder.Children)
            {
                FlattenNode(child, currentIndex); // Pass my index to my children
            }
        }
        else if (liveNode is FileNode file)
        {
            data.IsFolder = false;
            data.Type = file.Type;
            data.Content = file.Content;
            SaveData.Nodes.Add(data);
        }
    }

    private void LoadSystem()
    {
        this.Load("filesystem");

        if (SaveData.Nodes == null || SaveData.Nodes.Count == 0) return;

        // Array to temporarily hold our rebuilt objects so we can link them
        FSNode[] rebuiltNodes = new FSNode[SaveData.Nodes.Count];

        // First Pass: Create all the objects in memory
        for (int i = 0; i < SaveData.Nodes.Count; i++)
        {
            var data = SaveData.Nodes[i];
            if (data.IsFolder)
            {
                rebuiltNodes[i] = new FolderNode(data.Name, null);
            }
            else
            {
                rebuiltNodes[i] = new FileNode(data.Name, data.Type, null) { Content = data.Content };
            }
        }

        // Second Pass: Link Parents and Children using the ParentIndex
        for (int i = 0; i < SaveData.Nodes.Count; i++)
        {
            var data = SaveData.Nodes[i];
            if (data.ParentIndex != -1)
            {
                FolderNode parent = rebuiltNodes[data.ParentIndex] as FolderNode;
                FSNode child = rebuiltNodes[i];

                child.Parent = parent;
                parent.Children.Add(child);
            }
            else
            {
                // If ParentIndex is -1, this is our Root node (C:)
                Root = rebuiltNodes[i] as FolderNode;
            }
        }

        // Finally, grab the reference to the Desktop
        DesktopFolder = GetNodeByPath("C:/Desktop") as FolderNode;
    }

    private void CreateNewInstance()
    {
        Root = new FolderNode("C:", null);
        DesktopFolder = CreateFolder(Root, "Desktop");

        CreateFile(DesktopFolder, "testing", FileType.Text, "testing testing testing");
        FolderNode folder1 = CreateFolder(DesktopFolder, "testing1");
        FolderNode folder2 = CreateFolder(DesktopFolder, "testing2");
        FolderNode folder3 = CreateFolder(DesktopFolder, "testing3");

        CreateFile(folder1, "testing", FileType.Text, "testing testing testing");
        CreateFile(folder2, "testing", FileType.Text, "testing testing testing");
        CreateFile(folder3, "testing", FileType.Text, "testing testing testing");

        SaveSystem();
    }

    public FolderNode CreateFolder(FolderNode parent, string name)
    {
        FolderNode newFolder = new FolderNode(name, parent);
        parent.AddChild(newFolder);
        SaveSystem();
        return newFolder;
    }

    public FileNode CreateFile(FolderNode parent, string name, FileType type, string content = "")
    {
        FileNode newFile = new FileNode(name, type, parent) { Content = content };
        parent.AddChild(newFile);
        SaveSystem();
        return newFile;
    }
    public FileNode CreateFile(FolderNode parent, string name, Sprite image = null)
    {
        FileNode newFile = new FileNode(name, FileType.Image, parent)
        {
            Image = image // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAERLVBAE
        };

    public void UpdateContent(FileNode node, string newContent)
    {
        node.Content = newContent;
        SaveSystem();
    }

    public void DeleteNode(FSNode node)
    {
        if (node.Parent != null)
        {
            node.Parent.RemoveChild(node);
            SaveSystem();
        }
    }

    public void MoveNode(FSNode nodeToMove, FolderNode newParent)
    {
        if (nodeToMove.Parent != null)
        {
            nodeToMove.Parent.RemoveChild(nodeToMove);
        }
        newParent.AddChild(nodeToMove);
        SaveSystem();
    }

    public FSNode GetNodeByPath(string path)
    {
        string[] parts = path.Split('/');
        if (parts[0] != Root.Name) return null;

        FSNode currentNode = Root;
        for (int i = 1; i < parts.Length; i++)
        {
            if (currentNode is FolderNode folder)
            {
                currentNode = folder.Children.FirstOrDefault(c => c.Name == parts[i]);
                if (currentNode == null) return null;
            }
            else
            {
                return null;
            }
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