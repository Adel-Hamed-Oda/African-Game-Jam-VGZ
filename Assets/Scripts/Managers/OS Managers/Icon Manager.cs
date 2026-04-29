using UnityEngine;

public class IconManager : SingletonBehaviour<IconManager>
{
    [Header("System Icons")]
    public Sprite folderSprite;
    public Sprite textFileSprite;
    public Sprite unknownFileSprite; // A fallback icon

    // Returns the correct sprite based on the node type
    public Sprite GetIconForNode(FSNode node)
    {
        if (node is FolderNode)
        {
            return folderSprite;
        }
        else if (node is FileNode fileNode)
        {
            switch (fileNode.Type)
            {
                case FileType.Text:
                    return textFileSprite;
                // You can easily add FileType.Image etc. here later
                default:
                    return unknownFileSprite;
            }
        }

        return unknownFileSprite;
    }
}