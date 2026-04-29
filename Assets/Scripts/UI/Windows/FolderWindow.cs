using UnityEngine;

public class FolderWindow : Window
{
    [Header("Folder Specific Settings")]
    public Transform contentArea;
    public GameObject iconPrefab;

    private FolderNode currentFolder;

    public override void Initialize(FSNode node)
    {
        base.Initialize(node);

        currentFolder = node as FolderNode;
        if (currentFolder == null) return;

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