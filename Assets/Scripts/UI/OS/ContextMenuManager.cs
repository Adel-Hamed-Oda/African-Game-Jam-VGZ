using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ContextMenuManager : SingletonBehaviour<ContextMenuManager>
{
    [Header("References")]
    public GameObject menuObject;


[Header("Optional UI Groups")]
    public GameObject createGroup; // Create Text/Image/Folder
    public GameObject fileGroup;   // Rename/Delete

    private FSNode currentNode;         // null = desktop
    private FolderNode currentFolder;   // where new items will be created

    private bool isOpen = false;


    protected override void Awake()
    {
        base.Awake();
        Hide();
    }

    private void Update()
    {
        if (!isOpen) return;

        // Left click OR right click
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!IsPointerOverMenu())
            {
                Hide();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    // =========================
    // SHOW / HIDE
    // =========================

    public void Show(Vector2 screenPosition, FSNode node = null)
    {
        currentNode = node;

        if (node == null)
            currentFolder = FileSystemManager.Instance.DesktopFolder;
        else if (node is FolderNode folder)
            currentFolder = folder;
        else
            currentFolder = node.Parent;

        menuObject.SetActive(true);
        isOpen = true;

        RectTransform rect = menuObject.GetComponent<RectTransform>();
        rect.position = screenPosition;

        ConfigureOptions();
    }

    public void Hide()
    {
        if (menuObject != null)
        {       
            menuObject.SetActive(false);
            isOpen = false;
        }

        currentNode = null;
        currentFolder = null;
    }

    // =========================
    // UI CONFIG
    // =========================

    private void ConfigureOptions()
    {
        if (createGroup == null || fileGroup == null)
            return;

        bool isDesktop = currentNode == null;
        bool isFile = currentNode is FileNode;
        bool isFolder = currentNode is FolderNode;

        // Show create options on desktop or inside folders
        createGroup.SetActive(isDesktop || isFolder);

        // Show rename/delete on files or folders
        fileGroup.SetActive(isFile || isFolder);
    }

    // =========================
    // CREATE ACTIONS
    // =========================

    public void CreateText()
    {
        string name = FileSystemManager.Instance.GetUniqueName(currentFolder, "New Text");
        var tempNode = FileSystemManager.Instance.CreateFile(currentFolder, name, FileType.Text, "");
        Desktop.Instance.CreateIconAndRename(tempNode);
        Hide();
    }

    public void CreateImage()
    {
        string name = FileSystemManager.Instance.GetUniqueName(currentFolder, "New Text");
        var tempNode = FileSystemManager.Instance.CreateFile(currentFolder, name, FileType.Image, "Images/Input Prompts/keyboard");
        Desktop.Instance.CreateIconAndRename(tempNode);
        Hide();
    }

    public void CreateFolder()
    {
        string name = FileSystemManager.Instance.GetUniqueName(currentFolder, "New Text");
        var tempNode = FileSystemManager.Instance.CreateFolder(currentFolder, name);
        Desktop.Instance.CreateIconAndRename(tempNode);
        Hide();
    }

    // =========================
    // FILE ACTIONS
    // =========================

    public void Delete()
    {
        if (currentNode == null) return;

        FileSystemManager.Instance.DeleteNode(currentNode);

        Desktop.Instance.RefreshView();
        Hide();
    }

    public void Rename()
    {
        if (currentNode == null) return;

        IconUI icon = Desktop.Instance.FindIcon(currentNode);
        if (icon != null)
        {
            icon.StartRename();
        }

        Hide();
    }



    private bool IsPointerOverMenu()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == menuObject ||
                result.gameObject.transform.IsChildOf(menuObject.transform))
            {
                return true;
            }
        }

        return false;
    }

}


//public void Show(Vector2 position, FSNode node = null)
//{
//    currentNode = node;

//    // Determine context folder
//    if (node == null)
//    {
//        currentFolder = FileSystemManager.Instance.DesktopFolder;
//    }
//    else if (node is FolderNode folder)
//    {
//        currentFolder = folder;
//    }
//    else
//    {
//        currentFolder = node.Parent;
//    }

//    menuObject.SetActive(true);
//    menuObject.transform.position = position;

//    ConfigureOptions();
//}