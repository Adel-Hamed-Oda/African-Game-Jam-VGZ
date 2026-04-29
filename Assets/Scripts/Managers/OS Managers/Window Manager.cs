using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance;

    [Header("Steps")]
    public int dragStep;
    public int resizeStep;

    [Header("Window Prefabs")]
    public GameObject folderWindowPrefab;
    public GameObject textFileWindowPrefab;

    [Header("UI Parent")]
    public Transform desktopTransform;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenNode(FSNode node)
    {
        if (node is FolderNode folderNode)
        {
            // Spawn a Folder Window
            GameObject newWindow = Instantiate(folderWindowPrefab, desktopTransform);

            // Pass the folder data to the window's UI script
            FolderWindow folderUI = newWindow.GetComponent<FolderWindow>();
            folderUI.Initialize(folderNode);
        }
        else if (node is FileNode fileNode)
        {
            if (fileNode.Type == FileType.Text)
            {
                // Spawn a Text Editor Window
                GameObject newWindow = Instantiate(textFileWindowPrefab, desktopTransform);

                // Pass the file data to the text window's UI script
                TextWindow textUI = newWindow.GetComponent<TextWindow>();
                textUI.Initialize(fileNode);
            }
            // Add else-if here for FileType.Image in the future!
        }
    }
}