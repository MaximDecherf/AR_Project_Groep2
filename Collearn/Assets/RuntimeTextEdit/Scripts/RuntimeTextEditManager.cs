using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTE
{

  public class RuntimeTextEditManager : MonoBehaviour
  {
    public KeyCode armKey = KeyCode.RightControl;

    private List<RuntimeTextEdit> textEditComponents = new List<RuntimeTextEdit>();

    private bool isActive = false;

    private bool addMissingPhysicsRaycaster = false;

    public bool suppressAllConsoleOutput = false;

    public void RegisterComponent(RuntimeTextEdit comp)
    {
      textEditComponents.Add(comp);
    }

    public void Start()
    {
      if(Camera.main.GetComponent<Physics2DRaycaster>())
      {
        LogSystem.LogWithHighlight("Detected component Physics2DRaycaster on main camera. 3D text will not be clickable. Replace with PhysicsRaycaster.", Camera.main);
      }

      // This will detect both PhysicsRaycaster and Physics2DRaycaster
      if(Camera.main.GetComponent<PhysicsRaycaster>() == null)
      {
        string logMessage = "Could not find PhysicsRaycaster component on main camera. ";
        if(addMissingPhysicsRaycaster)
        {
          Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
          logMessage += "Added missing physics raycaster component to main camera. ";
        }
        else
        {
          logMessage += "Please add one.";
        }

        LogSystem.LogWithHighlight(logMessage, Camera.main);
      }
    }

    public void DeactivateMe(RuntimeTextEdit comp)
    {
      comp.enabled = false;
    }

    void Update()
    {
      if(Input.GetKey(armKey) && isActive == false)
      {
        isActive = true;
        foreach(RuntimeTextEdit comp in textEditComponents)
        {
          if(comp.enabled == false)
          {
            // @NOTE This causes OnEnable() to be called in RuntimeTextEdit.
            comp.enabled = true;
          }
          else
          {
            comp.ToggleShowRichTextTags();
          }
        }
      }
      else if(Input.GetKey(armKey) == false && isActive == true)
      {
        isActive = false;
        foreach(RuntimeTextEdit comp in textEditComponents)
        {
          if(comp.isActive() == false)
          {
            // @NOTE This causes OnDisable() to be called in RuntimeTextEdit.
            comp.enabled = false;
          }
        }
      }

      if(this.suppressAllConsoleOutput != LogSystem.suppressOutput)
      {
        LogSystem.suppressOutput = this.suppressAllConsoleOutput;
      }
    }
  }

  public class LogSystem
  {
    private static Queue pastLogTexts = new Queue();
    private static int maxNumOfPastLogTexts = 30;
    private static int blockLogOutputAtCount = 20;
    private static string consolePrefix = "RuntimeTextEdit: ";
    public static bool suppressOutput = false;

    public static void Log(string text)
    {
      if(ShouldLog(text))
      {
        Debug.Log(consolePrefix + text);
      }
    }

    public static void LogWarning(string text)
    {
      if(ShouldLog(text))
      {
        Debug.LogWarning(consolePrefix + text);
      }
    }

    public static void LogWithHighlight(string text, UnityEngine.Object linkedObject)
    {
      if(ShouldLog(text))
      {
        Debug.LogFormat(linkedObject, consolePrefix + text, new string[0]);
      }
    }

    private static bool ShouldLog(string text)
    {
      if(suppressOutput)
      {
        return false;
      }

      pastLogTexts.Enqueue(text);
      if(pastLogTexts.Count > maxNumOfPastLogTexts)
      {
        pastLogTexts.Dequeue();
      }
      int occurences = OccurencesInQueue(text, pastLogTexts);
      int OccurencesInQueue(string target, Queue queue)
      {
        int count = 0;
        foreach(string elem in queue)
        {
          if(elem == target)
          {
            count++;
          }
        }
        return count;
      }
      if(occurences == blockLogOutputAtCount)
      {
        Debug.Log(consolePrefix + "Prevented further outputting the following text because too many were generated: " + text);
        return false;
      }
      else if(occurences < blockLogOutputAtCount)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }

}
