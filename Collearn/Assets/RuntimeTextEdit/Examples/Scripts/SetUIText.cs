using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Loads and saves the text for two UI elements:
// 1. TextMesh Pro UI Text
// 2. Unity toggle
// Showcases how to use RuntimeTextEdit when multiple text components should be managed by one manager class.
public class SetUIText : MonoBehaviour, RTE.RuntimeTextEdit.TextEditCallback
{
  // TextMeshPro component
  public TMPro.TextMeshProUGUI textMeshProComponent;
  // JSON file path. Normally starts with Assets/...
  public string filePathTextMeshProText;

  // Unity text component
  public UnityEngine.UI.Text unityTextComponent;
  // JSON file path. Normally starts with Assets/...
  public string filePathUnityText;

  // Container struct for the text. JsonUtility requires a struct, class, ... and cannot work with a primitive type directly.
  [System.Serializable]
  private struct TextObject
  {
    public string text;
  }
  private TextObject textMeshProText;
  private TextObject unityText;

  // Loads text from the JSON file into the text component.
  void Start()
  {
    // TextMeshPro text
    string textMeshProJson = System.IO.File.ReadAllText(filePathTextMeshProText);
    textMeshProText = JsonUtility.FromJson<TextObject>(textMeshProJson);
    textMeshProComponent.text = textMeshProText.text;

    // Unity text
    string unityTextJson = System.IO.File.ReadAllText(filePathUnityText);
    unityText = JsonUtility.FromJson<TextObject>(unityTextJson);
    unityTextComponent.text = unityText.text;
  }

  // Called by RuntimeTextEdit when an edit was made. In this method the text is saved into the JSON file. textCompID is used to differentiate which text component was changed.
  public void PersistText(string newText, int textCompID)
  {
    // TextMeshPro component
    if(textMeshProComponent.GetInstanceID() == textCompID)
    {
      textMeshProText.text = newText;
      string json = JsonUtility.ToJson(textMeshProText);
      System.IO.File.WriteAllText(filePathTextMeshProText, json);
    }

    // Unity text component
    if(unityTextComponent.GetInstanceID() == textCompID)
    {
      unityText.text = newText;
      string json = JsonUtility.ToJson(unityText);
      System.IO.File.WriteAllText(filePathUnityText, json);
    }
  }
}
