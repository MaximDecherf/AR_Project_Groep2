﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetecter : MonoBehaviour
{

    Material m_Material;
    Material m_Collision;

    public GameObject targetObject;

    Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = targetObject.gameObject.GetComponent<Renderer>().material.color;
         //Fetch the Material from the Renderer of the GameObject
        m_Material = GetComponent<Renderer>().material;
        print (m_Material.color);
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
                m_Material.color = newColor;
                print(newColor);
            }
            Destroy(col.gameObject);
            
        }
    }
}
