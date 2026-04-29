using UnityEngine;

public class FolderWindow : Window
{
    [Header("Folder Specific Settings")]
    public Transform contentArea;
    public GameObject iconPrefab;

    private FolderNode currentFolder;

    public override void Initialize(FSNode node)
    {
        base.Initialize(node); // Calls the base class logic to set title

        currentFolder = node as FolderNode;
        if (currentFolder == null) return;

        // Overwrite the title to show the full path
        if (windowTitle != null)
        {
            windowTitle.text = currentFolder.GetPath();
        }

        RefreshView();
    }

    public void RefreshView()
    {
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        foreach (FSNode childNode in currentFolder.Children)
        {
            GameObject newIcon = Instantiate(iconPrefab, contentArea);
            newIcon.GetComponent<IconUI>().SetupIcon(childNode);
        }
    }
}