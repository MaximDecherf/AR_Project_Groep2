using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetecter : MonoBehaviour
{

    Material m_Material;
    Material m_Collision;
    Material start_Material;


    Color defaultColor = new Color(1,1,1,1);

    private GameManager gameManager;

    void Awake(){
        gameManager = GameObject.FindObjectOfType<GameManager> ();
    }

    // Start is called before the first frame update
    void Start()
    {
         //Fetch the Material from the Renderer of the GameObject
        m_Material = GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.name != null && col.gameObject.name != "Plane"){
            m_Collision =  col.gameObject.GetComponent<Renderer>().material;
            if (m_Material.color == defaultColor){
                m_Material.color = m_Collision.color;
            }
            else{
                Color newColor = ( m_Material.color + m_Collision.color) ;
                newColor.a = 1;
                m_Material.color = newColor;
                print(newColor);
            }
            gameManager.UpdateColor(m_Material.color);
            Destroy(col.gameObject);
            
        }
    }
}
