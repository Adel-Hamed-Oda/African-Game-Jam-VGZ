using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopRightClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ContextMenuManager.Instance.Show(eventData.position, null);
        }
    }
}