using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEditorInternal;
using TMPro;
using UnityEditor.Presets;
using UnityEditor;

namespace RTE
{

  public class RuntimeTextEdit : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
  {
    public GameObject textEditCallback;

    [Serializable]
    public class OnArm : UnityEvent {}
    [SerializeField]
    private OnArm onArm = new OnArm();

    [Serializable]
    public class OnDisarm : UnityEvent {}
    [SerializeField]
    private OnDisarm onDisarm = new OnDisarm();

    [Serializable]
    public class OnEnterEditMode : UnityEvent {}
    [SerializeField]
    private OnEnterEditMode onEnterEditMode = new OnEnterEditMode();

    [Serializable]
    public class OnLeaveEditMode : UnityEvent {}
    [SerializeField]
    private OnLeaveEditMode onLeaveEditMode = new OnLeaveEditMode();

    private static string packagePath = null;
    private static GameObject canvas = null;

    private TextComp textComp;
    private InputField inputField;

    private int textComponentID;

    //private TextEditCallback persistTextCallback;

    private bool isInEditMode = false;

    private static bool oneInstanceInEditMode = false;

    private RuntimeTextEditManager manager;

    // public bool disallowInvisibleText = true;

    private bool showRichTextTags = false;

    abstract class TextComp
    {
      protected string name;

      public abstract void OnEnterEditMode();
      public abstract void OnLeaveEditMode();
      public abstract InputField CreateInputField(UnityAction<string> editEndCallback);
      public abstract string GetText();
      public abstract void SetText(string newText);
      public abstract Transform GetTransform();
      public abstract int GetID();
      public abstract bool CanBeEditedInPlace();
      public abstract void Hide();
      public virtual Preset GetPresetOfInternalComp()
      {
        LogSystem.LogWarning("Called GetPresetOfInternalComp() on subclass that does not support it.");
        return null;
      }
      public virtual bool IsRaycastTarget()
      {
        return true;
      }
      public string GetGameobjectName()
      {
        return name;
      }
      public virtual void ShowRichTextTags(bool value)
      {
        // Default: Do nothing!
        return;
      }
    }

    class TextCompUnity3D : TextComp
    {
      private UnityEngine.TextMesh comp;

      private bool richTextStateBeforeEdit = false;

      public TextCompUnity3D(UnityEngine.TextMesh comp)
      {
        this.comp = comp;
        this.name = comp.gameObject.name;
      }

      public override void OnEnterEditMode()
      {
        richTextStateBeforeEdit = comp.richText;
      }

      public override void OnLeaveEditMode()
      {
        comp.transform.GetComponent<MeshRenderer>().enabled = true;
        comp.richText = richTextStateBeforeEdit;
      }

      public override InputField CreateInputField(UnityAction<string> editEndCallback)
      {
        return new InputFieldUnityFloat(this, editEndCallback);
      }

      public override string GetText()
      {
        return comp.text;
      }

      public override void SetText(string newText)
      {
        comp.text = newText;
      }

      public override Transform GetTransform()
      {
        return comp.transform;
      }

      public override int GetID()
      {
        return comp.GetInstanceID();
      }

      public override Preset GetPresetOfInternalComp()
      {
        return new Preset(comp);
      }

      public override bool CanBeEditedInPlace()
      {
        return false;
      }

      public override void Hide()
      {
        comp.transform.GetComponent<MeshRenderer>().enabled = false;
      }

      public override void ShowRichTextTags(bool value)
      {
        comp.richText = !value;
      }
    }

    class TextCompUnityUI : TextComp
    {
      private UnityEngine.UI.Text comp;

      public TextCompUnityUI(UnityEngine.UI.Text comp)
      {
        this.comp = comp;
        this.name = comp.gameObject.name;
      }

      public override void OnEnterEditMode()
      {

      }

      public override void OnLeaveEditMode()
      {
        comp.enabled = true;
      }

      public override InputField CreateInputField(UnityAction<string> editEndCallback)
      {
        return new InputFieldUnity(this, editEndCallback);
      }

      public override string GetText()
      {
        return comp.text;
      }

      public override void SetText(string newText)
      {
        comp.text = newText;
      }

      public override Transform GetTransform()
      {
        return comp.transform;
      }

      public override int GetID()
      {
        return comp.GetInstanceID();
      }

      public override Preset GetPresetOfInternalComp()
      {
        return new Preset(comp);
      }

      public override bool CanBeEditedInPlace()
      {
        return true;
      }

      public override void Hide()
      {
        comp.enabled = false;
      }

      public override bool IsRaycastTarget()
      {
        return comp.raycastTarget;
      }
    }

    class TextCompTextMeshProUGUI : TextComp
    {
      private TextMeshProUGUI comp;

      public TextCompTextMeshProUGUI(TextMeshProUGUI comp)
      {
        this.comp = comp;
        this.name = comp.gameObject.name;
      }

      public override void OnEnterEditMode()
      {

      }

      public override void OnLeaveEditMode()
      {
        comp.enabled = true;
      }

      public override InputField CreateInputField(UnityAction<string> editEndCallback)
      {
        return new InputFieldTextMeshPro(this, editEndCallback);
      }

      public override string GetText()
      {
        return comp.text;
      }

      public override void SetText(string newText)
      {
        comp.text = newText;
      }

      public override Transform GetTransform()
      {
        return comp.transform;
      }

      public override Preset GetPresetOfInternalComp()
      {
        return new Preset(comp);
      }

      public override int GetID()
      {
        return comp.GetInstanceID();
      }

      public override bool CanBeEditedInPlace()
      {
        return true;
      }

      public override void Hide()
      {
        comp.enabled = false;
      }

      public override bool IsRaycastTarget()
      {
        return comp.raycastTarget;
      }
    }

    class TextCompTextMeshPro : TextComp
    {
      private TextMeshPro comp;
      private bool richTextStateBeforeEdit = false;

      public TextCompTextMeshPro(TextMeshPro comp)
      {
        this.comp = comp;
        this.name = comp.gameObject.name;
      }

      public override void OnEnterEditMode()
      {
        richTextStateBeforeEdit = comp.richText;
      }

      public override void OnLeaveEditMode()
      {
        comp.enabled = true;
        comp.richText = richTextStateBeforeEdit;
      }

      public override InputField CreateInputField(UnityAction<string> editEndCallback)
      {
        return new InputFieldUnityFloat(this, editEndCallback);
      }

      public override string GetText()
      {
        return comp.text;
      }

      public override void SetText(string newText)
      {
        comp.text = newText;
      }

      public override Transform GetTransform()
      {
        return comp.transform;
      }

      public override int GetID()
      {
        return comp.GetInstanceID();
      }

      public override bool CanBeEditedInPlace()
      {
        return false;
      }

      public override void Hide()
      {
        comp.enabled = false;
      }

      public override void ShowRichTextTags(bool value)
      {
        comp.richText = !value;
      }
    }

    abstract class InputField
    {
      protected GameObject inputField;

      public abstract void SetText(string newText);
      public abstract string GetText();
      public virtual void SetParent(Transform parentTransform, bool keepWorldPos)
      {
        inputField.transform.SetParent(parentTransform, keepWorldPos);
      }

      public virtual void Remove()
      {
        Destroy(inputField);
      }

      public virtual void ShowRichTextTags(bool value)
      {
        // Default: do nothing
        return;
      }
    }

    class InputFieldUnity : InputField
    {
      protected UnityEngine.UI.InputField inputFieldComp;

      private TextComp linkedTextComp;

      public InputFieldUnity(TextComp textComp, UnityAction<string> editEndCallback)
      {
        GameObject inField = (GameObject)AssetDatabase.LoadAssetAtPath(RuntimeTextEdit.packagePath + "/Resources/InputField.prefab", typeof(GameObject));
        if(inField == null)
        {
          LogSystem.LogWarning("Failed to get input field prefab.");
          return;
        }
        this.inputField = Instantiate(inField);
        this.linkedTextComp = textComp;
        this.inputFieldComp = this.inputField.GetComponent<UnityEngine.UI.InputField>();
        this.inputFieldComp.onEndEdit.AddListener(editEndCallback);
        UnityAction<string> action = new UnityAction<string>(UpdateTextWhileTyping);
        this.inputFieldComp.onValueChanged.AddListener(action);

        if(textComp.GetType() == typeof(TextCompUnityUI))
        {
          MakeInputFieldHaveSameAppearance();
          void MakeInputFieldHaveSameAppearance()
          {
            Preset preset = textComp.GetPresetOfInternalComp();
            UnityEngine.UI.Text inputFieldTextComp = this.inputField.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
            preset.ApplyTo(inputFieldTextComp);
          }
        }

        return;
      }

      public override void SetText(string newText)
      {
        inputFieldComp.text = newText;
      }

      private void SetupRectComponent(ref RectTransform outRect, RectTransform inRect)
      {
        outRect.anchoredPosition3D = Vector3.zero;
        outRect.sizeDelta = inRect.sizeDelta;
        outRect.localScale = Vector3.one;
      }

      private void UpdateTextWhileTyping(string text)
      {
        this.linkedTextComp.SetText(text);
      }

      public override string GetText()
      {
        return inputFieldComp.text;
      }

      public override void ShowRichTextTags(bool value)
      {
        inputFieldComp.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().supportRichText = !value;
      }
    }

    class InputFieldTextMeshPro : InputField
    {
      private TMP_InputField tmpInputFieldComp;
      private TextComp linkedTextComp;

      public InputFieldTextMeshPro(TextComp comp, UnityAction<string> editEndCallback)
      {
        this.linkedTextComp = comp;

        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        inputField = TMP_DefaultControls.CreateInputField(resources);
        inputField.transform.SetParent(comp.GetTransform(), false);
        RectTransform inputFieldRectTrans = inputField.transform.GetComponent<RectTransform>();
        inputFieldRectTrans.anchorMin = Vector2.zero;
        inputFieldRectTrans.anchorMax = Vector2.one;
        inputFieldRectTrans.offsetMin = Vector2.zero;
        inputFieldRectTrans.offsetMax = Vector2.zero;

        tmpInputFieldComp = inputField.GetComponent<TMP_InputField>();
        UnityAction<string> action = new UnityAction<string>(UpdateTextWhileTyping);
        tmpInputFieldComp.onValueChanged.AddListener(action);

        // Copy text into input field
        tmpInputFieldComp.text = comp.GetText();

        tmpInputFieldComp.lineType = TMP_InputField.LineType.MultiLineNewline;

        // Disable background image
        inputField.GetComponent<Image>().enabled = false;

        // Create preset of text component property values
        TextMeshProUGUI textComp = inputField.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI placeholderTextComp = inputField.transform.Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>();
        string placeholderEmptyString = "Enter text...";
        Preset textCompPreset = (comp as TextCompTextMeshProUGUI).GetPresetOfInternalComp();
        bool success = textCompPreset.ApplyTo(textComp);
        success = textCompPreset.ApplyTo(placeholderTextComp);
        placeholderTextComp.text = placeholderEmptyString;
        placeholderTextComp.enabled = false;

        // Reset "Text Area" RectTransform component
        RectTransform textAreaRectTransform = inputField.transform.Find("Text Area").GetComponent<RectTransform>();
        Preset rectTransformResetter = new Preset(inputField.transform.Find("Text Area").Find("Text").GetComponent<RectTransform>());
        success = rectTransformResetter.ApplyTo(textAreaRectTransform);

        TMP_InputField inputFieldComp = inputField.GetComponent<TMP_InputField>();

        inputFieldComp.onEndEdit.AddListener(editEndCallback);

        // @NOTE This fixes caret not spawned when creating TextMeshPro input field during runtime.
        tmpInputFieldComp.enabled = false;
        tmpInputFieldComp.enabled = true;
      }

      public override void SetText(string newText)
      {
        tmpInputFieldComp.text = newText;
      }

      private void UpdateTextWhileTyping(string text)
      {
        this.linkedTextComp.SetText(text);
      }

      public override string GetText()
      {
        return tmpInputFieldComp.text;
      }

      public override void ShowRichTextTags(bool value)
      {
        tmpInputFieldComp.richText = true;
        tmpInputFieldComp.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().richText = !value;
      }
    }

    // Wrapper for InputFieldUnity that also renders a panel behind the text.
    class InputFieldUnityFloat : InputFieldUnity
    {
      private TextComp linkedTextComp;

      public InputFieldUnityFloat(TextComp textComp, UnityAction<string> editEndCallback) : base(textComp, editEndCallback)
      {
        this.linkedTextComp = textComp;

        UnityAction<string> action = new UnityAction<string>(UpdateTextWhileTyping);
        this.inputField.GetComponent<UnityEngine.UI.InputField>().onValueChanged.AddListener(action);

        string path = RuntimeTextEdit.packagePath + "/Resources/InputFieldPanel.prefab";
        GameObject panelPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        if(panelPrefab == null)
        {
          LogSystem.LogWarning("Failed to locate prefab at path: " + path);
          return;
        }
        GameObject panel = Instantiate(panelPrefab);
        this.inputField.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
        this.inputField.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);
        GameObject canvas = GetCanvas();
        panel.transform.SetParent(canvas.transform, false);
        this.inputField.transform.SetParent(panel.transform, false);
        this.inputField = panel;
      }

      private void UpdateTextWhileTyping(string text)
      {
        this.linkedTextComp.SetText(text);
      }

      public override string GetText()
      {
        return inputFieldComp.text;
      }
    }

    public interface TextEditCallback
    {
      void PersistText(string newText, int textCompID);
    }

    public static GameObject GetCanvas()
    {
      if(RuntimeTextEdit.canvas == null)
      {
        bool foundCanvas = SearchCanvas();
        bool SearchCanvas()
        {
          UnityEngine.Object canvasObj = UnityEngine.Object.FindObjectOfType(typeof(Canvas));
          if(canvasObj == null)
          {
            return false;
          }
          else
          {
            RuntimeTextEdit.canvas = ((Canvas)canvasObj).gameObject;
            return true;
          }
        }

        if(!foundCanvas)
        {
          CreateCanvas();
          void CreateCanvas()
          {
            GameObject canvasPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(RuntimeTextEdit.packagePath + "/Resources/Canvas.prefab", typeof(GameObject));
            if(canvasPrefab == null)
            {
              LogSystem.LogWarning("Could not find Canvas.prefab");
              return;
            }
            RuntimeTextEdit.canvas = Instantiate(canvasPrefab);
            GameObject eventSystemPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(RuntimeTextEdit.packagePath + "/Resources/EventSystem.prefab", typeof(GameObject));
            Instantiate(eventSystemPrefab);
          }
        }
      }

      return RuntimeTextEdit.canvas;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      // LogSystem.Log("OnPointerClick(...) in RuntimeTextEdit"); // @DELETE

      if(RuntimeTextEdit.oneInstanceInEditMode)
      {
        return;
      }

      if(textComp == null)
      {
        LogSystem.Log("Missing textComp!");
        return;
      }

      SpawnInputField();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    private void SpawnInputField()
    {
      this.isInEditMode = true;
      RuntimeTextEdit.oneInstanceInEditMode = true;

      onEnterEditMode.Invoke();

      inputField = textComp.CreateInputField(OnInputFieldEditExit);
      inputField.SetText(textComp.GetText());
      if(textComp.CanBeEditedInPlace())
      {
        textComp.Hide();
        inputField.SetParent(textComp.GetTransform(), false);
      }
      else
      {
        GameObject canvas = GetCanvas();
        inputField.SetParent(canvas.transform, false);
      }
      textComp.OnEnterEditMode();
    }

    public void OnInputFieldEditExit(string newText)
    {
      // LogSystem.Log("OnInputFieldEditExit(string newText) with text: " + newText);

      textComp.OnLeaveEditMode();
      textComp.SetText(newText);
      inputField.Remove();
      inputField = null;

      TextEditCallback persistTextCallback = textEditCallback.transform.GetComponent(typeof(RuntimeTextEdit.TextEditCallback)) as TextEditCallback;
      if(persistTextCallback != null)
      {
        persistTextCallback.PersistText(newText, textComp.GetID());
      }
      else
      {
        LogSystem.LogWithHighlight("Searched for implementation of interface TextEditCallback on GameObject (" + textEditCallback.name + ") but failed to find it. Could not call PersistText(...). Set inspector property TextEditCallback in RuntimeTextEdit.", this);
      }

      onLeaveEditMode.Invoke();

      this.isInEditMode = false;
      RuntimeTextEdit.oneInstanceInEditMode = false;
      manager.DeactivateMe(this);
    }

    void Start()
    {
      SearchManager();
      void SearchManager()
      {
        UnityEngine.Object manager = UnityEngine.Object.FindObjectOfType(typeof(RuntimeTextEditManager));
        if(manager == null)
        {
          LogSystem.LogWarning("Could not find RuntimeTextEditManager. Please add a RuntimeTextEditManager component to your scene. Can not continue.");
          return;
        }
        this.manager = ((RuntimeTextEditManager)manager);
        this.manager.RegisterComponent(this);
      }

      if(RuntimeTextEdit.packagePath == null)
      {
        RuntimeTextEdit.packagePath = "";
        LocatePackagePath();
      }

      SearchTextComp();
      void SearchTextComp()
      {
        bool foundTextComp = false;
        UnityEngine.UI.Text textCompUnityUI = this.transform.GetComponent<UnityEngine.UI.Text>();
        if(textCompUnityUI) {
          this.textComp = new TextCompUnityUI(textCompUnityUI);
          foundTextComp = true;
        }

        if(!foundTextComp) {
          TextMeshProUGUI textCompTextMeshProUGUI = this.transform.GetComponent<TextMeshProUGUI>();
          if(textCompTextMeshProUGUI) {
            this.textComp = new TextCompTextMeshProUGUI(textCompTextMeshProUGUI);
            foundTextComp = true;
          }
        }

        if(!foundTextComp)
        {
          TextMeshPro textCompTMP = this.transform.GetComponent<TextMeshPro>();
          if(textCompTMP)
          {
            this.textComp = new TextCompTextMeshPro(textCompTMP);
            Collider collider = textCompTMP.transform.GetComponent<Collider>();
            if(collider == null)
            {
              LogSystem.LogWithHighlight("Text component is missing a collider. Needed for RuntimeTextEdit to work. Added a box collider.", textCompTMP);
              BoxCollider boxCollider = textCompTMP.gameObject.AddComponent<BoxCollider>();
              boxCollider.center = Vector3.zero;
              boxCollider.isTrigger = true;
              RectTransform rectTrans = this.GetComponent<RectTransform>();
              if(rectTrans == null)
              {
                LogSystem.LogWarning("No RectTransform component found!");
                return;
              }
              boxCollider.size = new Vector3(rectTrans.sizeDelta.x, rectTrans.sizeDelta.y, 0);
            }
            foundTextComp = true;
          }
        }

        if(!foundTextComp)
        {
          UnityEngine.TextMesh textCompUnity3D = this.transform.GetComponent<UnityEngine.TextMesh>();
          if(textCompUnity3D)
          {
            this.textComp = new TextCompUnity3D(textCompUnity3D);
            Collider collider = textCompUnity3D.transform.GetComponent<Collider>();
            if(collider == null)
            {
              LogSystem.LogWithHighlight("Text component is missing a collider. Needed for RuntimeTextEdit to work. Added a box collider.", textCompUnity3D);
              BoxCollider boxCollider = textCompUnity3D.gameObject.AddComponent<BoxCollider>();
              boxCollider.isTrigger = true;
            }
            foundTextComp = true;
          }
        }

        if(!foundTextComp)
        {
          LogSystem.LogWithHighlight("Could not find text component in GameObject: " + this, this);
          return;
        }
      }

      void VerifyTextIsRaycastTarget()
      {
        if(textComp.IsRaycastTarget() == false)
        {
          LogSystem.LogWithHighlight("Text component on GameObject (" + textComp.GetGameobjectName() + ") is not a raycast target. Clicking on this text component will not trigger RuntimeTextEdit.", this);
          // @TODO If not a raycast target, register callback on OnArm and OnDisarm that makes it a raycasting target.
        }
      }
      VerifyTextIsRaycastTarget();

      /// Search for script that implements TextEditCallback
      SearchTextEditCallback();
      void SearchTextEditCallback()
      {
        if(textEditCallback == null)
        {
          textEditCallback = this.gameObject;
        }
      }

      manager.DeactivateMe(this);
    }

    public bool isActive()
    {
      return isInEditMode;
    }

    void Update()
    {
      if(this.isInEditMode)
      {
        if(this.inputField == null || this.textComp == null)
        {
          return;
        }
        if(this.inputField.GetText() != this.textComp.GetText())
        {
          LogSystem.LogWithHighlight("Text components text does not match input fields text. Maybe some other component changed the text component in its Update() method.", this);
        }
      }
    }

    public static string GetPackagePath()
    {
      if(RuntimeTextEdit.packagePath == null)
      {
        LocatePackagePath();
      }
      return RuntimeTextEdit.packagePath;
    }

    private static void LocatePackagePath()
    {
      if(Directory.Exists("Assets/RuntimeTextEdit"))
      {
        RuntimeTextEdit.packagePath = "Assets/RuntimeTextEdit";
      }
      else
      {
        string[] paths = Directory.GetDirectories("Assets", "RuntimeTextEdit", SearchOption.AllDirectories);
        if(paths.Length == 1)
        {
          RuntimeTextEdit.packagePath = paths[0];
        }
        else
        {
          LogSystem.LogWarning("Found more than one folder named RuntimeTextEdit in the project. Could not locate package path. Please rename all folders called RuntimeTextEdit that do not contain the RuntimeTextEdit package.");
        }
      }
    }

    public void ToggleShowRichTextTags()
    {
      showRichTextTags = !showRichTextTags;
      textComp.ShowRichTextTags(showRichTextTags);
      inputField.ShowRichTextTags(showRichTextTags);
    }

    public void OnEnable()
    {
      // LogSystem.Log("OnEnable()");

      onArm.Invoke();
    }

    public void OnDisable()
    {
      // LogSystem.Log("OnDisable()");

      onDisarm.Invoke();
    }

  }

}
