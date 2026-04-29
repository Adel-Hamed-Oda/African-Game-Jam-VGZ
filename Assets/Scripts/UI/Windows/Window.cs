using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class Window : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Window Settings")]
    public TextMeshProUGUI windowTitle;

    private RectTransform windowRectTransform;
    private int dragStep;
    private Vector2 _originalLocalPointerPosition;
    private Vector3 _originalPanelLocalPosition;

    protected FSNode currentNode; // Added to remember which node this window represents

    protected virtual void Awake()
    {
        windowRectTransform = GetComponent<RectTransform>();
        dragStep = WindowManager.Instance != null ? WindowManager.Instance.dragStep : 1;
    }

    public virtual void Initialize(FSNode node)
    {
        currentNode = node; // Save the node reference

        if (windowTitle != null)
        {
            windowTitle.text = node.Name;
        }
    }

    public void CloseWindow()
    {
        // Unregister from the WindowManager before destroying
        if (WindowManager.Instance != null && currentNode != null)
        {
            WindowManager.Instance.UnregisterWindow(currentNode);
        }

        Destroy(windowRectTransform.gameObject);
    }

    public void OnPointerDown(PointerEventData data)
    {
        windowRectTransform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)windowRectTransform.parent,
            data.position,
            data.pressEventCamera,
            out _originalLocalPointerPosition);

        _originalPanelLocalPosition = windowRectTransform.localPosition;
    }

    public void OnDrag(PointerEventData data)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)windowRectTransform.parent,
            data.position,
            data.pressEventCamera,
            out Vector2 localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - _originalLocalPointerPosition;
            Vector3 rawPosition = _originalPanelLocalPosition + offsetToOriginal;

            if (dragStep > 0f)
            {
                rawPosition.x = Mathf.Round(rawPosition.x / dragStep) * dragStep;
                rawPosition.y = Mathf.Round(rawPosition.y / dragStep) * dragStep;
            }

            windowRectTransform.localPosition = rawPosition;
        }
    }
}