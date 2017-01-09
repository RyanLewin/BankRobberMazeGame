using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    Map map;
    Vec2 size;
    UIController uiController;

    PlayerController player;
    public int level = 1;
    public int moves;
    
    public bool gameOverBool = false;
    public bool won = false;

    void Start()
    {
        map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        BeginGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            RestartGame();
    }

    public void MobileMove (string direction)
    {
        player.MobileDirection(direction);
    }

    void BeginGame()
    {
        if (won)
        {
            size = map.GetSize;
            level++;
            map.GetSize = new Vec2(size.x + 10, size.y + 10);
            uiController.UpdateLevel(level);
            won = false;
        }

        gameOverBool = false;
        map.Generate();
        player = map.GetPlayer;
    }

    public void RestartGame()
    {
        uiController.ResetUI();
        map.Reset();
        BeginGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public bool GameOverBool
    {
        get { return gameOverBool; }
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
