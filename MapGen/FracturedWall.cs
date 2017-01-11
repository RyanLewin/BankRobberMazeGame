using UnityEngine;
using System.Collections;

public class FracturedWall : MonoBehaviour {
    // Used to make the wall break into pieces when drilled
    
    float timer = 5f; //Time till broken wall despawn;
    GameManager gameManager;
    
    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    } 

    void Update ()
    {
        //Count down seconds till 0, or wait till game is over, then destroy the object
        timer -= Time.deltaTime;
        if (timer <= 0 || gameManager.GameOverBool)
            Destroy(gameObject);
    }
}
