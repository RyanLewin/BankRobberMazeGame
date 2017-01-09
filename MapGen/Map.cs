using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    public Vec2 size;
    public Cell mazeCellPrefab;
    private Cell[,] cellsArray;

    public MapFloor floorPrefab;
    public MapWall wallPrefab;
    public PlayerController playerPrefab;
    public EnemyController enemyPrefab;

    List<EnemyController> enemies;
    PlayerController player;

    public Material lastWall;

    public float delay = 0f;

    public void Generate()
    {
        cellsArray = new Cell[size.x, size.y];
        List<Cell> activeCells = new List<Cell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            DoNextGenerationStep(activeCells);
        }
        
        StartCoroutine(RevealTiles());

        Cell lastCell = cellsArray[Random.Range(0, size.x), size.y - 1];
        foreach (Transform t in lastCell.transform)
        {
            if (t.tag == "Wall")
            {
                MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                if (wallDir == MapDirection.North)
                {
                    t.GetChild(0).gameObject.GetComponent<Renderer>().material = lastWall;
                    break;
                }
            }
        }
        Debug.Log(lastCell.transform.name);
        BoxCollider bc = (BoxCollider)lastCell.gameObject.AddComponent(typeof(BoxCollider));
        bc.center = new Vector3(bc.center.x, 0.5f, bc.center.z);
        bc.isTrigger = true;

        enemies = new List<EnemyController>();
        for (int i = 0; i < (size.x + size.y) / 4; i++)
        {
            EnemyController enemy = Instantiate(enemyPrefab) as EnemyController;
            enemy.SetCell = cellsArray[Random.Range(0, size.x), Random.Range(5, size.y)];
            enemy.SetCellArray = cellsArray;
            enemies.Add(enemy);
        }

        Camera.main.transform.position = new Vector3(0.33f, 8f, -1.57f);
        Camera.main.transform.eulerAngles = new Vector3(75f, 0, 0);
        player = Instantiate(playerPrefab) as PlayerController;
        player.SetCell = cellsArray[0, 0];
        player.SetCellArray = cellsArray;
    }

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

    private void DoFirstGenerationStep (List<Cell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep (List<Cell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        Cell currCell = activeCells[currentIndex];

        if (currCell.IsFullyInitialised)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }

        MapDirection dir = currCell.RandomUninitialisedDirection;
        Vec2 coords = currCell.coordinates + dir.ToIntVec2();

        if (ContainsCoords(coords))
        {
            Cell neighbor = GetCell(coords);
            if (neighbor == null)
            {
                neighbor = CreateCell(coords);
                CreatePassage(currCell, neighbor, dir);
                activeCells.Add(neighbor);
            }
            else
            {
                CreateWall(currCell, neighbor, dir);
            }
        }
        else
        {
            CreateWall(currCell, null, dir);
        }
    }

    public void CreatePassage (Cell cell, Cell otherCell, MapDirection dir)
    {
        MapFloor floor = Instantiate(floorPrefab) as MapFloor;
        floor.Initialise(cell, otherCell, dir);
        floor = Instantiate(floorPrefab) as MapFloor;
        floor.Initialise(otherCell, cell, dir.GetOpposite());
    }

    void CreateWall (Cell cell, Cell otherCell, MapDirection dir)
    {
        MapWall wall = Instantiate(wallPrefab) as MapWall;
        wall.Initialise(cell, otherCell, dir);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MapWall;
            wall.Initialise(otherCell, cell, dir.GetOpposite());
        }
    }

    public Cell GetCell (Vec2 coords)
    {
        return cellsArray[coords.x, coords.y];
    }

    public Vec2 RandomCoordinates
    {
        get { return new Vec2(Random.Range(0, size.x), Random.Range(0, size.y)); }
    }

    public bool ContainsCoords (Vec2 coord)
    {
        return coord.x >= 0 && coord.x < size.x && coord.y >= 0 && coord.y < size.y;
    }

    Cell CreateCell(Vec2 coords)
    {
        Cell cell = Instantiate(mazeCellPrefab);
        cellsArray[coords.x, coords.y] = cell;
        cell.coordinates = coords;
        cell.name = "Floor [" + coords.x + "," + coords.y + "]";
        cell.transform.parent = transform;
        cell.transform.localPosition = new Vector3(coords.x + 0.5f, -0.1f, coords.y + 0.5f);
        cell.gameObject.SetActive(false);
        return cell;
    }

    public void Reset()
    {
        Destroy(player.gameObject);
        foreach (Cell cell in cellsArray)
        {
            Destroy(cell.gameObject);
        }

        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
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
