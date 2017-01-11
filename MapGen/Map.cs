//Credit to Catlike Coding for the map generation tutorial,
//http://catlikecoding.com/unity/tutorials/maze/
//mine is slightly edited for my game to work but the tut was a huge help

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    public Vec2 size; //size of the map

    //Cell object and array of
    public Cell mazeCellPrefab; 
    private Cell[,] cellsArray;

    public MapFloor passagePrefab; //Passage
    public MapWall wallPrefab; //Wall
    public PlayerController playerPrefab; //Player
    public EnemyController enemyPrefab; //Enemy

    [SerializeField]
    List<EnemyController> enemies;
    PlayerController player;

    public Material lastWall;

    public float delay = 0f;

    //Generation of the map
    public void Generate()
    {
        //Tell the script to create the map of size size.x by size.y,
        //default start of 20x20.
        cellsArray = new Cell[size.x, size.y];
        List<Cell> activeCells = new List<Cell>();

        //Create all the cells until there are no active cells left
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
            DoNextGenerationStep(activeCells);
        
        //Reveal all the tiles
        StartCoroutine(RevealTiles());

        //Pick a cell from the last row that will be the finish point
        Cell lastCell = cellsArray[Random.Range(0, size.x), size.y - 1];
        foreach (Transform t in lastCell.transform)
        {
            if (t.tag == "Wall")
            {
                MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                //Select the wall on the north side
                if (wallDir == MapDirection.North)
                {
                    //Currently just changing the material
                    t.GetChild(0).gameObject.GetComponent<Renderer>().material = lastWall;
                    break;
                }
            }
        }
        Debug.Log(lastCell.transform.name);

        //Add a collider to the last cell that will trigger the game end once the player enters
        BoxCollider bc = (BoxCollider)lastCell.gameObject.AddComponent(typeof(BoxCollider));
        bc.center = new Vector3(bc.center.x, 0.5f, bc.center.z);
        bc.isTrigger = true;

        //Create a list of enemies, size = map size.x + size.y / 4 
        //eg. if x and y were both 20, the no of enemies would be 20 + 20 / 4 = 10
        enemies = new List<EnemyController>();
        for (int i = 0; i < (size.x + size.y) / 4; i++)
        {
            EnemyController enemy = Instantiate(enemyPrefab) as EnemyController;
            //spawn the enemies but set them to spawn at least 5 spaces to the right so they don't ruin
            //the players chances too quick
            enemy.SetCell = cellsArray[Random.Range(0, size.x), Random.Range(5, size.y)];
            enemy.SetCellArray = cellsArray;
            enemies.Add(enemy);
        }

        //Set the cameras position and rotation
        Camera.main.transform.position = new Vector3(0.33f, 8f, -1.57f);
        Camera.main.transform.eulerAngles = new Vector3(75f, 0, 0);
        //Instantiate player in the bottom-left cell
        player = Instantiate(playerPrefab) as PlayerController;
        player.SetCell = cellsArray[0, 0];
        player.SetCellArray = cellsArray;
    }

    //Reveal tiles in columns from left to right, just for fun
    public IEnumerator RevealTiles ()
    {
        WaitForSeconds delayFloat = new WaitForSeconds(delay);
        yield return delayFloat;
        for (int x = 0; x<size.x; x++)
        {
            yield return delayFloat;
            for (int y = 0; y<size.y; y++)
            {
                cellsArray[x, y].gameObject.SetActive(true);
            }
        }

    }

    //Create the first cell and add it to the active cells
    private void DoFirstGenerationStep (List<Cell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    //This method is used to retrieve the current cell, 
    //check whether a move is possible and remove if its fully initialised.
    private void DoNextGenerationStep (List<Cell> activeCells)
    {
        //Set currCell to the last active cell
        int currentIndex = activeCells.Count - 1;
        Cell currCell = activeCells[currentIndex];

        //Remove cell from active cells when all sides are Initialised
        if (currCell.IsFullyInitialised)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }

        //Set a random map direction
        MapDirection dir = currCell.RandomUninitialisedDirection;
        //set coords to the correct positon
        Vec2 coords = currCell.coordinates + dir.ToIntVec2();

        //Check that the coords are actually viable, eg. the x value should be over 0 but below the size.x
        if (ContainsCoords(coords))
        {
            //Set the neighbour cell to the cell at coords
            Cell neighbor = GetCell(coords);
            if (neighbor == null)
            {
                //Create the cell at the set coords
                neighbor = CreateCell(coords);
                //Create passage between the current cell and its neighbour
                CreatePassage(currCell, neighbor, dir);
                activeCells.Add(neighbor);
            }
            else
            {
                //If the neighbor is not part of the same passage then create a wall between them
                CreateWall(currCell, neighbor, dir);
            }
        }
        else
        {
            //Creates the wall around the perimiter
            CreateWall(currCell, null, dir);
        }
    }

    //Create passage between the current cell and the neighbor
    public void CreatePassage (Cell cell, Cell otherCell, MapDirection dir)
    {
        //Instantiate the passage in the cell's position, facing the other cell
        MapFloor passage = Instantiate(passagePrefab) as MapFloor;
        passage.Initialise(cell, otherCell, dir);
        //Instantiate another passage in the other cell's position, facing the current cell
        passage = Instantiate(passagePrefab) as MapFloor;
        passage.Initialise(otherCell, cell, dir.GetOpposite());
    }

    //Create wall between the current cell and the other cell, called if other cell is null
    void CreateWall (Cell cell, Cell otherCell, MapDirection dir)
    {
        //Instantiate a wall on the correct side of the cell
        MapWall wall = Instantiate(wallPrefab) as MapWall;
        wall.Initialise(cell, otherCell, dir);
        //Instantiate a wall on the opposite side of the other cell if it isn't null
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MapWall;
            wall.Initialise(otherCell, cell, dir.GetOpposite());
        }
    }

    //Return the cell at the given coords
    public Cell GetCell (Vec2 coords)
    {
        return cellsArray[coords.x, coords.y];
    }

    //Return Random coords
    public Vec2 RandomCoordinates
    {
        get { return new Vec2(Random.Range(0, size.x), Random.Range(0, size.y)); }
    }

    //Check if coords are in the bounds of the map
    public bool ContainsCoords (Vec2 coord)
    {
        return coord.x >= 0 && coord.x < size.x && coord.y >= 0 && coord.y < size.y;
    }

    //Create a cell which will contain the floor the player stands on
    Cell CreateCell(Vec2 coords)
    {
        Cell cell = Instantiate(mazeCellPrefab);
        //Set the correct cell in the cellsArray to the just instantiated cell
        cellsArray[coords.x, coords.y] = cell;
        cell.coordinates = coords;
        cell.name = "Floor [" + coords.x + "," + coords.y + "]";
        cell.transform.parent = transform;
        //Postioning of cells
        cell.transform.localPosition = new Vector3(coords.x + 0.5f, -0.1f, coords.y + 0.5f);
        cell.gameObject.SetActive(false);
        return cell;
    }

    //Reset the map
    public void Reset()
    {
        //Destroy the player
        Destroy(player.gameObject);
        //Destroy each cell
        foreach (Cell cell in cellsArray)
        {
            Destroy(cell.gameObject);
        }
        //Destroy each enemy
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
        //Stop all coroutines in case map is still being generated whilst reset is calledZ
        StopAllCoroutines();
    }

    public Vec2 GetSize
    {
        get { return size; }
        set { size = value; }
    }

    public List<EnemyController> GetEnemies
    {
        get { return enemies; }
    }

    public PlayerController GetPlayer
    {
        get { return player; }
    }
}
