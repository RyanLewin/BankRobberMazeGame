using UnityEngine;
using System.Collections;

public class Pawn : MonoBehaviour {

    protected Cell[,] cells; //Multi-Dimensional array of all cells

    [SerializeField]
    protected Cell currCell; //Current cell
    protected Cell otherCell; //neighbor cell

    protected Vec2 otherPos; //Positon of other cell
    [SerializeField]
    protected Vec2 pos; //current position

    [SerializeField]
    protected MapDirection dir; //direction currently facing, ie. North, East, South, West

    protected Map map;
    protected UIController uiController;
    protected GameManager gameManager;
    protected bool stop = false; //Should the enemy be paused


    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //set the pos to the current cell's coords then move them to the right place
        pos = currCell.coordinates;
        transform.position = cells[pos.x, pos.y].transform.position;
        transform.localPosition = new Vector3(transform.position.x, transform.lossyScale.y, transform.position.z);
        map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        //Set parent to the current cell
        transform.parent = currCell.transform;
        dir = MapDirection.North;
    }

    //Check if there is a wall in front of the pawn
    protected bool CheckForWall()
    {
        foreach (Transform t in currCell.transform)
        {
            if (t.tag == "Wall")
            {
                MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                if (dir == wallDir)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Check that the pawn won't be going outside the boundaries of the map
    protected bool InRange()
    {
        if (pos.y == 0 && dir == MapDirection.South)
            return false;
        else if (pos.y == map.size.y - 1 && dir == MapDirection.North)
            return false;
        else if (pos.x == 0 && dir == MapDirection.West)
            return false;
        else if (pos.x == map.size.x - 1 && dir == MapDirection.East)
            return false;
        else
            return true;
    }

    //Check whether the pawn is able to move
    protected bool Move()
    {
        do
        {
            if (CheckForWall())
                return false;
            if (!InRange())
                return false;
        } while (false);

        otherPos = currCell.coordinates + dir.ToIntVec2();
        otherCell = cells[otherPos.x, otherPos.y];

        //Check otherCell to see if it contains an enemy or player, if the enemy
        //sees the player in the other cell they will stop to "take a shot"
        for (int i = 0; i < otherCell.transform.childCount; i++)
        {
            string tag = otherCell.transform.GetChild(i).tag;
            if (tag == "Enemy")
                return false;
            else if (tag == "Player")
            {
                stop = true;
                return false;
            }
        }

        //move position of pawn and update parentz
        transform.position += dir.ToVec3();
        transform.parent = otherCell.transform;
        return true;
    }

    public Cell SetCell
    {
        set { currCell = value; }
    }
    public Cell[,] SetCellArray
    {
        set { cells = value; }
    }
}
