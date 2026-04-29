using UnityEngine;
using TMPro;

public class TextWindow : Window
{
    [Header("Text Editor Settings")]
    public TMP_InputField contentInput;

    private FileNode currentFile;

    public override void Initialize(FSNode node)
    {
        base.Initialize(node);

        currentFile = node as FileNode;
        if (currentFile == null) return;

        if (windowTitle != null)
        {
            windowTitle.text = currentFile.Name;
        }

        contentInput.text = currentFile.Content;

        contentInput.onValueChanged.RemoveAllListeners();
        contentInput.onValueChanged.AddListener(OnTextChanged);
    }

    private void OnTextChanged(string newText)
    {
        if (currentFile != null)
        {
            currentFile.Content = newText;
        }
    }
}