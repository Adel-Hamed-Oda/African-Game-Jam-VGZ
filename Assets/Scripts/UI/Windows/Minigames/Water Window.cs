using UnityEngine;
using UnityEngine.UI;

public class WaterWindow : ExecutableWindow
{
    [Header("Minigame Viewport")]
    [Tooltip("The UI RawImage that will display the game")]
    public RawImage viewport;

    [Tooltip("The prefab containing your level, player, and a Camera")]
    public GameObject minigameWorldPrefab;

    [Header("Camera Settings")]
    [Tooltip("How many UI pixels equals 1 Unity unit? Increase to zoom out, decrease to zoom in.")]
    public float pixelsPerUnit = 32f;

    private GameObject activeWorld;
    private Camera minigameCamera;
    private RenderTexture renderTexture;

    private Vector2 lastViewportSize; // Used to track resizing

    public override void Initialize(FSNode node)
    {
        base.Initialize(node);

        if (windowTitle != null) windowTitle.text = node.Name;

        LaunchGame();
    }

    private void LaunchGame()
    {
        // 1. Instantiate the game world far away
        Vector3 hiddenLocation = new Vector3(10000, 10000, 0);
        activeWorld = Instantiate(minigameWorldPrefab, hiddenLocation, Quaternion.identity);

        // 2. Find the camera inside the spawned minigame
        minigameCamera = activeWorld.GetComponentInChildren<Camera>();
        if (minigameCamera == null)
        {
            Debug.LogError("No Camera found inside the Minigame World Prefab!");
            return;
        }

        // Ensure it's an orthographic camera for 2D
        minigameCamera.orthographic = true;

        // 3. Do an initial texture build
        CheckAndResizeViewport();
    }

    private void Update()
    {
        // Keep checking if the player resized the window
        CheckAndResizeViewport();
    }

    private void CheckAndResizeViewport()
    {
        if (viewport == null || minigameCamera == null) return;

        // Get the current pixel size of the RawImage UI element
        Vector2 currentSize = viewport.rectTransform.rect.size;

        // If the size changed (and is valid), rebuild the texture and camera size
        if (currentSize != lastViewportSize && currentSize.x > 0 && currentSize.y > 0)
        {
            UpdateRenderTexture((int)currentSize.x, (int)currentSize.y);
            lastViewportSize = currentSize;
        }
    }

    private void UpdateRenderTexture(int width, int height)
    {
        // 1. Clean up the old texture if it exists
        if (renderTexture != null)
        {
            minigameCamera.targetTexture = null;
            viewport.texture = null;
            renderTexture.Release();
            Destroy(renderTexture);
        }

        // 2. Create the new texture matching the exact new UI dimensions
        renderTexture = new RenderTexture(width, height, 24);
        renderTexture.filterMode = FilterMode.Point; // Keeps pixel art crisp!

        // 3. Link the new texture to the Camera and the UI
        minigameCamera.targetTexture = renderTexture;
        viewport.texture = renderTexture;

        // 4. Adjust the camera's zoom so 1 Unity Unit = 'pixelsPerUnit' UI Pixels
        // This ensures the game reveals MORE of the level when the window is maximized, 
        // rather than stretching a small image!
        minigameCamera.orthographicSize = (height / 2f) / pixelsPerUnit;
    }

    private void OnDestroy()
    {
        if (activeWorld != null)
        {
            Destroy(activeWorld);
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}