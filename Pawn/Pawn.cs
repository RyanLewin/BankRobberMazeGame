using UnityEngine;
using System.Collections;

public class Pawn : MonoBehaviour {

    protected Cell[,] cells;

    [SerializeField]
    protected Cell currCell;
    protected Cell otherCell;

    protected Vec2 otherPos;
    [SerializeField]
    protected Vec2 pos;

    [SerializeField]
    protected MapDirection dir;

    protected Map map;
    protected UIController uiController;
    protected GameManager gameManager;
    protected bool stop = false;


    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        pos = currCell.coordinates;
        transform.position = cells[pos.x, pos.y].transform.position;
        transform.localPosition = new Vector3(transform.position.x, transform.lossyScale.y, transform.position.z);
        map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        transform.parent = currCell.transform;
        dir = MapDirection.North;
    }

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
