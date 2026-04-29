using UnityEngine;
using UnityEngine.EventSystems;

public class WindowResizer : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Tooltip("Minimum size the window can be resized to.")]
    public Vector2 minSize = new(200f, 150f);
    
    private RectTransform windowRectTransform;
    private int resizeStep;
    private Vector2 _originalLocalPointerPosition;
    private Vector2 _originalSizeDelta;

    private void Awake()
    {
        windowRectTransform = GetComponentInParent<Window>().GetComponent<RectTransform>();

        resizeStep = WindowManager.Instance != null ? WindowManager.Instance.resizeStep : 1;
    }

    public void OnPointerDown(PointerEventData data)
    {
        windowRectTransform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowRectTransform,
            data.position,
            data.pressEventCamera,
            out _originalLocalPointerPosition);

        _originalSizeDelta = windowRectTransform.sizeDelta;
    }
    public void OnDrag(PointerEventData data)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowRectTransform,
            data.position,
            data.pressEventCamera,
            out Vector2 localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - _originalLocalPointerPosition;

            // Calculate the raw new size based on mouse movement
            Vector2 rawSize = _originalSizeDelta + new Vector2(offsetToOriginal.x, -offsetToOriginal.y);

            // Snap the size to the specified increments (e.g., nearest 50)
            if (resizeStep > 0f)
            {
                rawSize.x = Mathf.Round(rawSize.x / resizeStep) * resizeStep;
                rawSize.y = Mathf.Round(rawSize.y / resizeStep) * resizeStep;
            }

            // Clamp to minimum size
            rawSize.x = Mathf.Max(rawSize.x, minSize.x);
            rawSize.y = Mathf.Max(rawSize.y, minSize.y);

            windowRectTransform.sizeDelta = rawSize;
        }
    }
}