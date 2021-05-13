using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsScript : MonoBehaviour
{

    public static int Points = 0;
    public Text PointsText;


    // Start is called before the first frame update
    void Start()
    {
        PointsText.text = Points.ToString();
    }


    public void LevelComplete(){
        Points += 100;
        PointsText.text = Points.ToString();
    }
}
