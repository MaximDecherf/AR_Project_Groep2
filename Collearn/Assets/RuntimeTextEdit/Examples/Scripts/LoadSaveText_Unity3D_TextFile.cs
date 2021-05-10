using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Loads text from a text file into the text component at startup and saves the text there when PersistText(...) is called.
public class LoadSaveText_Unity3D_TextFile : MonoBehaviour, RTE.RuntimeTextEdit.TextEditCallback
{
  public string textFilePath;

  // Called by RuntimeTextEdit when an edit was made. In this method the text is saved into the text file.
  public void PersistText(string newText, int textCompID)
  {
    System.IO.File.WriteAllText(textFilePath, newText);
  }

  // Loads text from the text file into the text component.
  void Start()
  {
    string textFromFile = System.IO.File.ReadAllText(textFilePath);
    UnityEngine.TextMesh textComp = this.transform.GetComponent<UnityEngine.TextMesh>();
    textComp.text = textFromFile;
  }
}
