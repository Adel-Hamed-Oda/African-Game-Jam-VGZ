using UnityEngine;
using UnityEngine.UI;

public class ImageWindow : Window
{
    [Header("Image Settings")]
    public Image imageDisplay;

    private FileNode currentFile;

    public override void Initialize(FSNode node)
    {
        base.Initialize(node);

        currentFile = node as FileNode;
        if (currentFile == null) return;

        if (windowTitle != null)
        {
            windowTitle.text = currentFile.Name;
        }

        imageDisplay.sprite = ImageManager.Instance.GetImageByName(currentFile.Content);
    }
}