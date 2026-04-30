using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileSystemManager : SingletonBehaviour<FileSystemManager>, ISerializable<FileSystemManager.FSData>
{
    public class FSData
    {
        public FolderNode Root;
    }
    public FSData SaveData { get; set; }

    public FolderNode Root { get; private set; }
    public FolderNode DesktopFolder { get; private set; } // Added reference to the Desktop

    public Sprite defaultImage; // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAE

    void Start()
    {
        Root = new FolderNode("C:", null);

        DesktopFolder = CreateFolder(Root, "Desktop");

        CreateFile(DesktopFolder, "testing", "testing testing testing testing testing");
        FolderNode folder1 = CreateFolder(DesktopFolder, "testing1");
        FolderNode folder2 = CreateFolder(DesktopFolder, "testing2");
        FolderNode folder3 = CreateFolder(DesktopFolder, "testing3");
        CreateFile(DesktopFolder, "imagetesting",  defaultImage);
        CreateFile(folder1, "testing", "testing testing testing testing testing");
        CreateFile(folder2, "testing", "testing testing testing testing testing");
        CreateFile(folder3, "testing", "testing testing testing testing testing");

        // 3. Initialize the Desktop UI
        if (Desktop.Instance != null)
        {
            Desktop.Instance.Initialize(DesktopFolder);
        }
    }

    public FolderNode CreateFolder(FolderNode parent, string name)
    {
        FolderNode newFolder = new FolderNode(name, parent);
        parent.AddChild(newFolder);
        return newFolder;
    }
    public FileNode CreateFile(FolderNode parent, string name, string content = "")
    {
        FileNode newFile = new FileNode(name, FileType.Text, parent)
        {
            Content = content,
        }; 

        parent.AddChild(newFile);
        return newFile;
    }
    public FileNode CreateFile(FolderNode parent, string name, Sprite image = null)
    {
        FileNode newFile = new FileNode(name, FileType.Image, parent)
        {
            Image = image // IMAGE WNLVAUIWNRLNVERAHBVHABLERJVBAERLVBAE
        };

        parent.AddChild(newFile);
        return newFile;
    }
    public void DeleteNode(FSNode node)
    {
        if (node.Parent != null)
        {
            node.Parent.RemoveChild(node);
        }
    }
    public void MoveNode(FSNode nodeToMove, FolderNode newParent)
    {
        if (nodeToMove.Parent != null)
        {
            nodeToMove.Parent.RemoveChild(nodeToMove);
        }
        newParent.AddChild(nodeToMove);
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
                if (currentNode == null) return null; // Path broken
            }
            else
            {
                return null; // Hit a file before the path ended
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