using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Loads text from a JSON file into the text component at startup and saves the text there when PersistText(...) is called.
public class LoadSaveText_UnityUI_JSON : MonoBehaviour, RTE.RuntimeTextEdit.TextEditCallback
{
  // JSON file path. Normally starts with Assets/...
  public string filePath;

  // Container struct for the text. JsonUtility requires a struct, class, ... and cannot work with a primitive type directly.
  [System.Serializable]
  private struct TextObject
  {
    public string text;
  }
  private TextObject textObject;

  // Loads text from the JSON file into the text component.
  void Start()
  {
    string json = System.IO.File.ReadAllText(filePath);
    textObject = JsonUtility.FromJson<TextObject>(json);
    this.gameObject.GetComponent<UnityEngine.UI.Text>().text = textObject.text;
  }

  // Called by RuntimeTextEdit when an edit was made. In this method the text is saved into the JSON file.
  public void PersistText(string newText, int textCompID)
  {
    textObject.text = newText;
    string json = JsonUtility.ToJson(textObject);
    System.IO.File.WriteAllText(filePath, json);
  }
}
