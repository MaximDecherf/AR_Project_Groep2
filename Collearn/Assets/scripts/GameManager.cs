using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int baseHardness = 2;

    public GameObject ColorSphere;
    Color levelColor;



    // Start is called before the first frame update
    void Start()
    {
        Vector3 ground = GameObject.Find("Plane").GetComponent<Renderer>().bounds.size;

        float ground_width = ground.x/3;
        float ground_length = ground.z/3;


        int levelHardness = SceneManager.GetActiveScene().buildIndex;
        SpawnLevel((baseHardness + levelHardness), ground_width, ground_length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SpawnLevel(int levelHardness, float boundary_x, float boundary_y){
        print(boundary_x);
        print(boundary_y);
        print(levelHardness);
        
        
        
        float r = 0.007f;
        float g = 0.420f;
        float b = 0.696f;

        levelColor = new Color(r, g, b, 1);

        for (int i = 0 ; i < levelHardness; ++i) {
            Vector3 position = new Vector3(Random.Range(-boundary_x, boundary_x), 0.5f, Random.Range(-boundary_y, boundary_y));
            var Sphere = Instantiate(ColorSphere, position, Quaternion.identity);
            Material m_color = Sphere.gameObject.GetComponent<Renderer>().material;
        
            m_color.color = new Color(r, g, b, 1);
        }
        
        
    }
}
