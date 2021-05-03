using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int baseHardness = 5;

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
        float r = Random.Range(0.000f, 1f);
        float g = Random.Range(0.000f, 1f);
        float b = Random.Range(0.000f, 1f);
        
        levelColor = new Color(r, g, b, 1);

        print("The level color is: " + levelColor);

        List<Color> solution = SolutionMaker(levelColor);


        for (int i = 0 ; i < levelHardness; ++i) {
            Vector3 position = new Vector3(Random.Range(-boundary_x, boundary_x), 0.5f, 0);
            var Sphere = Instantiate(ColorSphere, position, Quaternion.identity);
            Material m_color = Sphere.gameObject.GetComponent<Renderer>().material;
        
            if(solution.Count > i){
                m_color.color = solution[i];
                print(solution[i]);
            }
            else{
                float rRandom = Random.Range(0.000f, 1f);
                float gRandom = Random.Range(0.000f, 1f);
                float bRandom = Random.Range(0.000f, 1f);

                m_color.color = new Color(rRandom, gRandom, bRandom, 1);
            }
           
        }
        
        
    }

    List<Color> SolutionMaker(Color color){

        List<Color> solution = new List<Color>();

        while(solution.Count < 2)
        {
        float r = Random.Range(0.000f, 1f);
        float g = Random.Range(0.000f, 1f);
        float b = Random.Range(0.000f, 1f);
        
        if((ColorChecker(color[0], r) != 0f) && (ColorChecker(color[1], g) != 0f) && (ColorChecker(color[2], b) != 0f)){
            Color newColor = new Color(r, g, b);
            solution.Add(newColor);
        }
        }
        
        return solution;
    }
    float ColorChecker(float cSol, float cNew){
        if (cSol + cNew < 1f){
            return cNew;
        }
        else if (cSol - cNew > 0f){
            return -cNew;
        }
        else{
            return 0f;
        }
    }
}
