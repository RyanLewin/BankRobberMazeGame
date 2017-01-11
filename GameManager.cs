using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    Map map;
    UIController uiController;
    PlayerController player;

    Vec2 size; //Size of map
    public int level = 1; //level that the player is on
    public int moves; //No of moves the player has done
    
    public bool gameOverBool = false; //Has the player been killed or quit?
    public bool won = false; //Has the player got the finish?

    void Start()
    {
        map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        BeginGame();
    }

    void Update()
    {
        //Just for testing, to reset the map
        if (Input.GetKeyDown(KeyCode.Return))
            RestartGame();
    }

    //testing for mobile controls
    public void MobileMove (string direction)
    {
        player.MobileDirection(direction);
    }

    //Begin the game
    void BeginGame()
    {
        // If the player won the last level, increase the size of the map by 10 each way,
        // increase the level by 1 and update the UI for level
        if (won)
        {
            size = map.GetSize;
            level++;
            map.GetSize = new Vec2(size.x + 10, size.y + 10);
            uiController.UpdateLevel(level);
            won = false;
        }

        gameOverBool = false;
        //Call for the map to generate
        map.Generate();
        player = map.GetPlayer;
    }

    //Reset the UI and the map, then begin the game again
    public void RestartGame()
    {
        uiController.ResetUI();
        map.Reset();
        BeginGame();
    }

    //Quit the application
    public void Quit()
    {
        Application.Quit();
    }

    public bool GameOverBool
    {
        get { return gameOverBool; }
        //set gameOverBool and if set to true, reset the level to 1 and moves to 0
        set
        {
            gameOverBool = value;
            if (gameOverBool)
            {
                level = 1;
                moves = 0;
                uiController.GameOver();
            }
        }
    }

    public bool GetWon
    {
        get { return won; }
        set { won = value; }
    }

    public int GetMoves
    {
        get { return moves; }
        set { moves = value; }
    }
}
