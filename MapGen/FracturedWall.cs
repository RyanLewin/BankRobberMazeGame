using UnityEngine;
using System.Collections;

public class FracturedWall : MonoBehaviour {

    float timer = 5f;
    GameManager gameManager;
    
    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    } 

    void Update ()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 || gameManager.GameOverBool)
            Destroy(gameObject);
    }
}
