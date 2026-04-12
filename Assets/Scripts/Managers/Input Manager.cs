using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, ISerializable<InputManager.InputSaveData>
{
    public static InputManager Instance { get; private set; }

    [SerializeField] public InputActionAsset inputActions;

    private const string SaveKey = "input_bindings";

    [Serializable]
    public class InputSaveData
    {
        public string bindingOverrides = "";
    }
    public InputSaveData SaveData { get; set; } = new InputSaveData();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        this.Load(SaveKey);
        ApplyBindingOverrides();
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Starts an interactive rebind for one binding on the given action.
    /// </summary>
    /// <param name="action">The InputAction to rebind (e.g. inputActions["Jump"]).</param>
    /// <param name="bindingIndex">Index of the binding to replace (usually 0).</param>
    /// <param name="onComplete">Optional callback fired when the player picks a new key.</param>
    /// <param name="onCancel">Optional callback fired when the player presses Escape.</param>
    public void Rebind(InputAction action,
                       int bindingIndex,
                       Action onComplete = null,
                       Action onCancel = null)
    {
        // The action must be disabled while rebinding.
        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
              .WithCancelingThrough("<Keyboard>/escape")
              .OnComplete(op =>
              {
                  op.Dispose();
                  action.Enable();
                  PersistBindings();
                  onComplete?.Invoke();
              })
              .OnCancel(op =>
              {
                  op.Dispose();
                  action.Enable();
                  onCancel?.Invoke();
              })
              .Start();
    }

    /// <summary>
    /// Wipes every player override and saves the clean state.
    /// Bind this to your "Reset to Defaults" button in the UI.
    /// </summary>
    public void ResetToDefaults()
    {
        inputActions.RemoveAllBindingOverrides();
        SaveData = new InputSaveData();   // empty string → no overrides
        this.Save(SaveKey);
        Debug.Log("Input bindings reset to defaults.");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private void ApplyBindingOverrides()
    {
        if (!string.IsNullOrEmpty(SaveData.bindingOverrides))
            inputActions.LoadBindingOverridesFromJson(SaveData.bindingOverrides);
    }

    private void PersistBindings()
    {
        SaveData.bindingOverrides = inputActions.SaveBindingOverridesAsJson();
        this.Save(SaveKey);
    }
}