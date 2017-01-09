using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

    public Vec2 coordinates;
    private int initialisedEdgeCount;

    private MapCellEdge[] edges = new MapCellEdge[MapDirections.count];
    
    GameManager gameManager;

    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void OnTriggerEnter (Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            gameManager.GetWon = true;
            gameManager.RestartGame();
        }
    }

    public bool IsFullyInitialised
    {
        get { return initialisedEdgeCount == MapDirections.count; }
    }

    public MapDirection RandomUninitialisedDirection
    {
        get
        {
            int skips = Random.Range(0, MapDirections.count - initialisedEdgeCount);
            for (int i = 0; i < MapDirections.count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                    {
                        return (MapDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialised directions left.");
        }
    }

    public MapCellEdge GetEdge (MapDirection dir)
    {
        return edges[(int)dir];
    }

    public void SetEdge (MapDirection dir, MapCellEdge edge)
    {
        edges[(int)dir] = edge;
        initialisedEdgeCount += 1;
    }
}
