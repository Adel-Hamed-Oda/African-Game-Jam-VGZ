using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // Added for the window title

[RequireComponent(typeof(RectTransform))]
public class Window : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Window Settings")]
    public TextMeshProUGUI windowTitle;

    private RectTransform windowRectTransform;
    private int dragStep;
    private Vector2 _originalLocalPointerPosition;
    private Vector3 _originalPanelLocalPosition;

    protected virtual void Awake() // Made virtual in case child classes need Awake
    {
        windowRectTransform = GetComponent<RectTransform>();

        // Assuming you add 'public int dragStep = 1;' to your WindowManager script!
        dragStep = WindowManager.Instance != null ? WindowManager.Instance.dragStep : 1;
    }

    // Virtual so child classes can override and add their own logic
    public virtual void Initialize(FSNode node)
    {
        // Default behavior: just set the title to the file/folder name
        if (windowTitle != null)
        {
            windowTitle.text = node.Name;
        }
    }

    public void CloseWindow()
    {
        Destroy(windowRectTransform.gameObject);
    }

    public void OnPointerDown(PointerEventData data)
    {
        // Bring the window to the front
        windowRectTransform.SetAsLastSibling();

        // Record the starting positions for dragging
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

            // Snap the position to the specified increments
            if (dragStep > 0f)
            {
                rawPosition.x = Mathf.Round(rawPosition.x / dragStep) * dragStep;
                rawPosition.y = Mathf.Round(rawPosition.y / dragStep) * dragStep;
            }

            windowRectTransform.localPosition = rawPosition;
        }
    }
}