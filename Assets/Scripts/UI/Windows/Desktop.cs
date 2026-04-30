using UnityEngine;

public class Desktop : MonoBehaviour
{
    public static Desktop Instance;

    [Header("Desktop Settings")]
    public Transform contentArea; // The fullscreen panel with a GridLayoutGroup
    public GameObject iconPrefab; // Your existing Icon Prefab

    private FolderNode desktopFolder;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(FolderNode folder)
    {
        desktopFolder = folder;
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
        RefreshView();
    }

    public void RefreshView()
    {
        // Clear existing icons
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Spawn icons for the desktop folder
        foreach (FSNode childNode in desktopFolder.Children)
        {
            GameObject newIcon = Instantiate(iconPrefab, contentArea);
            newIcon.GetComponent<IconUI>().SetupIcon(childNode);
        }
    }

    public IconUI CreateIconAndRename(FSNode node)
    {
        GameObject newIcon = Instantiate(iconPrefab, contentArea);
        IconUI icon = newIcon.GetComponent<IconUI>();

        icon.SetupIcon(node);
        icon.StartRename();

        //RefreshView();

        return icon;
    }

    public IconUI FindIcon(FSNode node)
    {
        foreach (Transform child in contentArea)
        {
            IconUI icon = child.GetComponent<IconUI>();
            if (icon != null && icon.CurrentNode == node)
            {
                return icon;
            }
        }
        return null;
    }
}