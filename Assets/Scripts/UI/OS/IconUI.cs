using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IconUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public Image iconImage;

    private FSNode currentNode;

    public void SetupIcon(FSNode node)
    {
        currentNode = node;
        nameText.text = node.Name;

        iconImage.sprite =  ImageManager.Instance.GetImageForNode(node);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check for double click
        if (eventData.clickCount == 2)
        {
            WindowManager.Instance.OpenNode(currentNode);
        }
    }
}