using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    Text,
    Image
}
public class FileNode : FSNode
{
    public FileType Type { get; set; }
    public string Content { get; set; }

    public FileNode(string name, FolderNode parent)
    {
        Name = name;
        Parent = parent;
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

public class FileSystemManager : SingletonBehaviour<FileSystemManager>
{
    public FolderNode Root { get; private set; }

    void Start()
    {
        Root = new FolderNode("C:", null);

        CreateFile(Root, "Readme.txt", "Welcome to Unity OS!");

        Debug.Log("Created file at: " + GetNodeByPath("C:/Readme.txt")?.GetPath());
    }

    public FolderNode CreateFolder(FolderNode parent, string name)
    {
        FolderNode newFolder = new FolderNode(name, parent);
        parent.AddChild(newFolder);
        return newFolder;
    }
    public FileNode CreateFile(FolderNode parent, string name, string content = "")
    {
        FileNode newFile = new FileNode(name, parent) { Content = content };
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