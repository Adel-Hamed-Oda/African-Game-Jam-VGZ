using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//using UnityEditor.Experimental.GraphView;

public class IconUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public Image iconImage;

    private FSNode currentNode;

    public TMP_InputField renameInput;

    public void SetupIcon(FSNode node)
    {
        currentNode = node;
        nameText.text = node.Name;

        iconImage.sprite =  ImageManager.Instance.GetImageForNode(node);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ContextMenuManager.Instance.Show(eventData.position, currentNode);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
        {
            WindowManager.Instance.OpenNode(currentNode);
            return;
        }
    }

    public void StartRename()
    {
        nameText.gameObject.SetActive(false);
        renameInput.gameObject.SetActive(true);

        renameInput.text = currentNode.Name;
        renameInput.Select();
        renameInput.ActivateInputField();

        renameInput.onEndEdit.RemoveAllListeners();
        renameInput.onEndEdit.AddListener(OnRenameFinished);
    }

    private void OnRenameFinished(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            currentNode.Name = newName;
            FileSystemManager.Instance.Save("filesystem");
        }

        renameInput.gameObject.SetActive(false);
        nameText.gameObject.SetActive(true);
        nameText.text = currentNode.Name;
    }
}