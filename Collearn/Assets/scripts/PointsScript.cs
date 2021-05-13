using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsScript : MonoBehaviour
{

    public static int points = 0;
    public Text pointsText;


    // Start is called before the first frame update
    void Start()
    {
        pointsText.text = points.ToString();
    }


    public void LevelComplete(){
        points += 100;
        pointsText.text = points.ToString();
    }
}
