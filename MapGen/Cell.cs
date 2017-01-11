using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

    public Vec2 coordinates; //Coordinates of the cell
    private int initialisedEdgeCount; //How many of the bordering cells are initialised

    private MapCellEdge[] edges = new MapCellEdge[MapDirections.count]; //Array of edges

    //If the player enters the collider box, which is only on the final cell,
    //the level is won and the game restarted
    void OnTriggerEnter (Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            gameManager.GetWon = true;
            gameManager.RestartGame();
        }
    }

    //Check whether all the edges are initialised
    public bool IsFullyInitialised
    {
        get { return initialisedEdgeCount == MapDirections.count; }
    }

    //Return a random direction as long as there is still a direction available
    public MapDirection RandomUninitialisedDirection
    {
        get
        {
            //Get a random number between 0 and 4-initialisedEdgeCount
            int skips = Random.Range(0, MapDirections.count - initialisedEdgeCount);
            for (int i = 0; i < MapDirections.count; i++)
            {
                //if the current edge is null and skips is 0 then return the ith mapdirection
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

    //Set edge and increase the init edge count
    public void SetEdge (MapDirection dir, MapCellEdge edge)
    {
        edges[(int)dir] = edge;
        initialisedEdgeCount += 1;
    }
}
