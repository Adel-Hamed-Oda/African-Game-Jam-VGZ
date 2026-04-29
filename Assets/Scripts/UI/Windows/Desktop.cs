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
}