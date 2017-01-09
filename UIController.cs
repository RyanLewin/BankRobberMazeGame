using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Transform canvas;
    Transform UI;
    Transform gun;
    Transform knife;
    Transform drills;
    Transform gameOver;
    Text movesText;
    Text timeText;
    Text levelText;

    int level = 1;
    int moves;
    Time time;

    GameManager gameManager;

    public string finTime;
    public string finMoves;

    void Start ()
    {
        UI = canvas.GetChild(1);
        gameOver = canvas.GetChild(0);
        gun = UI.GetChild(1);
        knife = UI.GetChild(2);
        drills = UI.GetChild(3);
        movesText = UI.GetChild(4).GetComponent<Text>();
        timeText = UI.GetChild(5).GetComponent<Text>();
        levelText = UI.GetChild(6).GetComponent<Text>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
        {
            knife.gameObject.SetActive(false);
            drills.gameObject.SetActive(false);
            gun.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            knife.gameObject.SetActive(true);
            drills.gameObject.SetActive(false);
            gun.gameObject.SetActive(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
        {
            knife.gameObject.SetActive(false);
            drills.gameObject.SetActive(true);
            gun.gameObject.SetActive(false);
        }

        if (gameManager.GameOverBool)
        {
            float angle = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, -Camera.main.transform.eulerAngles.x, Time.deltaTime);
            Camera.main.transform.eulerAngles = new Vector3(angle, 0, 0);
            gameOver.GetChild(1).GetComponent<Text>().text = finMoves;
            gameOver.GetChild(2).GetComponent<Text>().text = "Time: " + finTime;
            gameOver.GetChild(3).GetComponent<Text>().text = "Level: " + level;
        }
        else
        {
            timeText.text = "Time: " + Time.time.ToString("#.##");
        }
    }

    public void Moves (int moves)
    {
        movesText.text = "Moves: " + moves;
    }

    public void WeaponUI (string weapon, int a = 3)
    {
        if (weapon == drills.name)
        {
            for (int i = a; i >= 0; i--)
            {
                if (drills.GetChild(i).name.Contains(i.ToString()))
                {
                    Image image = drills.GetChild(i).GetComponent<Image>();
                    Color color = image.color;

                    color = new Color(color.r, color.g, color.b, 0.3f);
                    image.color = color;
                    break;
                }
            }
        }
    }

    public void ResetUI ()
    {
        gameOver.gameObject.SetActive(false);
        finMoves = "";
        finTime = "";
        UI.gameObject.SetActive(true);

        foreach (Transform drill in drills)
        {
            Image image = drill.GetComponent<Image>();
            Color color = image.color;
            color = new Color(color.r, color.g, color.b, 1f);
            image.color = color;
        }
    }

    public void UpdateLevel (int _level)
    {
        level = _level;
        levelText.text = "Level: " + level;
    }

    public void GameOver ()
    {
        UI.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(true);
        finMoves = movesText.text;
        finTime = Time.time.ToString("#.##");
    }
}
