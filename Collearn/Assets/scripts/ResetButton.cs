using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{

    public Button resetButton;	

    private GameManager gameManager;
    private ColliderDetecter targetObject;

    void Awake(){
        gameManager = GameObject.FindObjectOfType<GameManager> ();
        targetObject = GameObject.FindObjectOfType<ColliderDetecter> ();
    }

 
    void Start () {
		Button btn = resetButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}	
    void TaskOnClick(){
		print("MAX MAX MAX SUPER MAX MAX MAX SUPER MAX MAX MAX");
        gameManager.ResetLevel();
        targetObject.ResetColor();
	}

}
