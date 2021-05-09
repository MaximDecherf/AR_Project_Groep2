using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    int baseHardness = 2;

    public GameObject ColorSphere;
    Color levelColor;
    Dictionary<Color, List<Color>> solutions = new Dictionary<Color, List<Color>>();

    


    // Start is called before the first frame update
    void Start()
    {
        initSolutions();
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

    public void UpdateColor(Color color){
        if (color == levelColor){
            print("level complete");
        }
    }


    void SpawnLevel(int levelHardness, float boundary_x, float boundary_y){
    
        int flag = Random.Range(0, solutions.Count - 2);
        int index = 0;
        
        foreach(var color in solutions){
            if(index == flag){
                levelColor = color.Key;  
            }
            index++;
        }

        List<Color> solution = solutions[levelColor];

        print("The level color is: " + levelColor);

        


        for (int i = 0 ; i < levelHardness; ++i) {
            Vector3 position = new Vector3(Random.Range(-boundary_x, boundary_x), 0.5f, 0f);
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

    void initSolutions(){
        solutions.Add(new Color(1, 0, 1, 1), new List<Color>(){new Color(1, 0, 0, 1), new Color(0, 0, 1, 1)}); //magenta
        solutions.Add(new Color(1, 0.92f, 0.016f, 1), new List<Color>(){new Color(1, 0, 0, 1), new Color(0, 0.92f, 0.016f, 1)}); // Yellow
        solutions.Add(new Color(0, 1, 1, 1), new List<Color>(){new Color(0, 1, 0, 1), new Color(0, 0, 1, 1)}); // cyan
        solutions.Add(new Color(0.22f, 0, 1, 1), new List<Color>(){new Color(0.11f, 0, 0.5f, 1), new Color(0.11f, 0, 0.5f, 1)}); // random from here on
        solutions.Add(new Color(0, 0.8f, 0.4f, 1), new List<Color>(){new Color(0, 0.8f, 0, 1), new Color(0, 0, 0.4f, 1)});
        solutions.Add(new Color(1, 0.2f, 0.05f, 1), new List<Color>(){new Color(1, 0, 0, 1), new Color(0, 0.2f, 0.05f, 1)});
        solutions.Add(new Color(0.6f, 0.420f, 0, 1), new List<Color>(){new Color(0.3f, 0, 0, 1), new Color(0.3f, 0.420f, 0, 1)});
        solutions.Add(new Color(0.9f, 1, 0.2f, 1), new List<Color>(){new Color(0.45f, 0, 0, 1), new Color(0.45f, 1, 0.2f, 1)});
        solutions.Add(new Color(0, 1, 0.5f, 1), new List<Color>(){new Color(0, 0.5f, 0.1f, 1), new Color(0, 0.5f, 0.4f, 1)});
        solutions.Add(new Color(0.9f, 0.555f, 0.23f, 1), new List<Color>(){new Color(0.9f, 0, 0, 1 ), new Color(0, 0.555f, 0, 1), new Color(0, 0, 0.23f, 1)});
        solutions.Add(new Color(1, 1, 1, 1), new List<Color>(){new Color(1, 0, 0, 1), new Color(0, 1, 0, 1), new Color(0, 0, 1, 1 )}); // white
    }
}
