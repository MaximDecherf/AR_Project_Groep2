using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RTE;
using System;

public class Character : MonoBehaviour, RuntimeTextEdit.TextEditCallback
{
  public string monologueJsonFile;
  public string jumpTextFile;

  private Rigidbody rigidbodyComp;
  public TextMeshPro textComp;
  private GameObject continueButton;
  private string jumpText;

  private int positionInMonologue = 0;
  private int[] stopDialogIndices = {2, 3, 13};

  [System.Serializable]
  private struct Monologue
  {
    public string[] lines;
  }
  private Monologue mono;

  public enum State
  {
    JUMPING,
    RESTING,
  }

  private State state;

  public void PersistText(string newText, int textCompID)
  {
    if(this.state == State.JUMPING)
    {
      System.IO.File.WriteAllText(jumpTextFile, newText);
      jumpText = newText;
      textComp.text = newText;
    }
    else if(this.state == State.RESTING)
    {
      mono.lines[positionInMonologue] = newText;
      string json = JsonUtility.ToJson(mono);
      System.IO.File.WriteAllText(monologueJsonFile, json);
    }
  }

  void Start()
  {
    this.rigidbodyComp = this.GetComponent<Rigidbody>();
    state = State.JUMPING;
    jumpText = System.IO.File.ReadAllText(jumpTextFile);
    textComp.text = jumpText;
    continueButton = this.transform.Find("Continue Button").gameObject;

    string json = System.IO.File.ReadAllText(monologueJsonFile);
    mono.lines = new string[0]; // TODO does this work?
    mono = JsonUtility.FromJson<Monologue>(json);
  }

  public void Jump()
  {
    this.rigidbodyComp.AddForce(Vector3.up * 100);
  }

  public void PauseSimulation()
  {
    this.rigidbodyComp.constraints = RigidbodyConstraints.FreezeAll;
  }

  public void ResumeSimulation()
  {
    this.rigidbodyComp.constraints = RigidbodyConstraints.FreezeRotation;
  }

  public void UserChangedCharacterText()
  {
    if(positionInMonologue == 13)
    {
      AdvanceText();
    }
  }

  public void UserStartedEdit()
  {
    if(positionInMonologue == 2)
    {
      AdvanceText();
    }
  }

  public void UserEndedEdit()
  {
    if(positionInMonologue == 3)
    {
      AdvanceText();
    }
  }

  public void AdvanceText()
  {
    int maxIndexInArray = mono.lines.Length - 1;
    if(positionInMonologue + 1 <= maxIndexInArray)
    {
      positionInMonologue++;
      textComp.text = mono.lines[positionInMonologue];
    }
    if(Array.Exists(stopDialogIndices, elem => elem == positionInMonologue) || positionInMonologue == maxIndexInArray)
    {
      HideContinueButton();
    }
    else
    {
      ShowContinueButton();
    }
  }

  public void HideContinueButton()
  {
    continueButton.SetActive(false);
  }

  public void ShowContinueButton()
  {
    continueButton.SetActive(true);
  }

  void OnCollisionEnter(Collision collision)
  {
    // Debug.Log("OnCollisionEnter()");
    state = State.RESTING;
    textComp.text = mono.lines[positionInMonologue];
  }

  void OnCollisionExit(Collision collision)
  {
    // Debug.Log("OnCollisionExit()");
    state = State.JUMPING;
    textComp.text = jumpText;
  }
}
