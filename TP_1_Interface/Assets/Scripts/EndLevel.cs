using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isLimit;
    public string nameNextLevel;
    public string nameReloadLevel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            if (isLimit)
            {
                SceneManager.LoadScene(nameReloadLevel);
            }
            else
            {
                SceneManager.LoadScene(nameNextLevel);
            }
        }
    }
}
