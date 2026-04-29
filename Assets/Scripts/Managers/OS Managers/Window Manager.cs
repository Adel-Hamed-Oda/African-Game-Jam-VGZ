using System.Collections.Generic;
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

    // Dictionary to keep track of which nodes already have an open window
    private Dictionary<FSNode, Window> openWindows = new Dictionary<FSNode, Window>();

    private void Awake()
    {
        Instance = this;
    }

    public void OpenNode(FSNode node)
    {
        // 1. Check if the window is already open
        if (openWindows.ContainsKey(node))
        {
            if (openWindows[node] != null)
            {
                // Bring the existing window to the front and exit
                openWindows[node].GetComponent<RectTransform>().SetAsLastSibling();
                return;
            }
            else
            {
                // Fallback: If the window was destroyed unexpectedly, remove it from the list
                openWindows.Remove(node);
            }
        }

        // 2. If not open, create the window
        Window newWindowScript = null;

        if (node is FolderNode folderNode)
        {
            GameObject newWindow = Instantiate(folderWindowPrefab, desktopTransform);
            newWindowScript = newWindow.GetComponent<FolderWindow>();
            newWindowScript.Initialize(folderNode);
        }
        else if (node is FileNode fileNode)
        {
            if (fileNode.Type == FileType.Text)
            {
                GameObject newWindow = Instantiate(textFileWindowPrefab, desktopTransform);
                newWindowScript = newWindow.GetComponent<TextWindow>();
                newWindowScript.Initialize(fileNode);
            }
        }

        // 3. Register the newly created window
        if (newWindowScript != null)
        {
            openWindows.Add(node, newWindowScript);
        }
    }

    // Called by the window when it is closed
    public void UnregisterWindow(FSNode node)
    {
        if (node != null && openWindows.ContainsKey(node))
        {
            openWindows.Remove(node);
        }
    }
}