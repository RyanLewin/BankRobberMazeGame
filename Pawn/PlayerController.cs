using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Pawn
{
    public Transform mobile;
    Weapon weapon; //Which weapon is the player using

    int drillsUsed = 0; //no of drills used
    public const int totalDrills = 3;// total number of drills available
    string direction = ""; //direction of the player, used for mobile testing
    bool move = false; //Has the player tried to move?

    void Update()
    {
        //Check if game is over
        if (!gameManager.GameOverBool)
        {
            //Check if input for north, east, south or west, then set dir and rotation accordingly
            //If the PlayerMove() returns true then change their pos and currCell
            if (Input.GetKeyDown(KeyCode.W) || direction == "North")
            {
                dir = (MapDirection)0;
                transform.rotation = dir.ToRotation();

                if (PlayerMove())
                {
                    pos.y++;
                    currCell = cells[pos.x, pos.y];
                }
                move = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) || direction == "East")
            {
                dir = (MapDirection)1;
                transform.rotation = dir.ToRotation();
                if (PlayerMove())
                {
                    pos.x++;
                    currCell = cells[pos.x, pos.y];
                }
                move = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) || direction == "South")
            {
                dir = (MapDirection)2;
                transform.rotation = dir.ToRotation();
                if (PlayerMove())
                {
                    pos.y--;
                    currCell = cells[pos.x, pos.y];
                }
                move = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) || direction == "West")
            {
                dir = (MapDirection)3;
                transform.rotation = dir.ToRotation();
                if (PlayerMove())
                {
                    pos.x--;
                    currCell = cells[pos.x, pos.y];
                }
                move = true;
            }
            //Reset direction (for mobile use)
            direction = "";

            //Check if player wants to change weapon/tool in use
            if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
                weapon = (Weapon)0;
            else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
                weapon = (Weapon)1;
            else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
                weapon = (Weapon)2;

            //Check for use of weapon
            if (Input.GetKeyUp(KeyCode.Space))
            {
                switch (weapon)
                {
                    case (Weapon.Gun):
                        Shoot();
                        break;
                    case (Weapon.Knife):
                        Stab();
                        break;
                    case (Weapon.Drill):
                        //Check the player has drills available
                        if (drillsUsed < totalDrills)
                            Drill();
                        break;
                }
            }
        }
    }

    public void MobileDirection (string _direction)
    {
        direction = _direction;
    }

    void Shoot()
    {
        //Create a raycast forwards up to 15 metres and check if it hit an enemy, 
        //if it did, destroy the enemy object and remove him from the enemies list
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.ToVec3(), out hit, 15f))
        {
            if (hit.transform.tag == "Enemy")
            {
                Destroy(hit.transform.gameObject);
                map.GetEnemies.Remove(hit.transform.GetComponent<EnemyController>());
            }
        }
    }

    void Stab()
    {
        //If there is a wall between them then return
        foreach (Transform t in currCell.transform)
            if (t.tag == "Wall")
            {
                MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                if (dir == wallDir)
                    return;
            }

        //Get the other cell
        otherPos = currCell.coordinates + dir.ToIntVec2();
        otherCell = cells[otherPos.x, otherPos.y];

        //if the other cell contains an enemy, destroy the object and remove him from the
        //enemies list
        for (int i = 0; i < otherCell.transform.childCount; i++)
        {
            if (otherCell.transform.GetChild(i).tag == "Enemy")
            {
                Destroy(otherCell.transform.GetChild(i).gameObject);
                map.GetEnemies.Remove(otherCell.transform.GetChild(i).gameObject.GetComponent<EnemyController>());
            }
        }
    }

    void Drill()
    {
        //Make sure player isn't trying to destroy an outer wall to escape the map
        if (InRange())
        {
            //Check each child in the current cell for a wall
            foreach (Transform t in currCell.transform)
            {
                if (t.tag == "Wall")
                {
                    MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                    //If there is a wall in the right direction, destroy it 
                    //and instantiate the broken wall in its position
                    if (dir == wallDir)
                    {
                        Instantiate(Resources.Load("Prefabs/Map_Wall_Frac"), t.position, t.rotation);
                        Destroy(t.gameObject);
                        //Add one to drills used and update the UI
                        drillsUsed++;
                        uiController.WeaponUI("Drills", totalDrills - drillsUsed);
                        
                        //Get the other cell to destroy the opposing wall
                        otherPos = currCell.coordinates + dir.ToIntVec2();
                        otherCell = cells[otherPos.x, otherPos.y];

                        foreach (Transform a in otherCell.transform)
                        {
                            if (a.tag == "Wall")
                            {
                                wallDir = a.GetComponent<MapCellEdge>().dir.GetOpposite();
                                if (dir == wallDir)
                                {
                                    Destroy(a.gameObject);
                                    return;
                                    //lots of closing brackets
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //Is the player able to move
    bool PlayerMove()
    {
        //If move returns false, then return false
        if (!Move())
            return false;

        //Increase manager moves and update UI
        gameManager.GetMoves++;
        uiController.Moves(gameManager.GetMoves);

        //Transform the camera's position to follow player and return true
        Camera.main.transform.position += dir.ToVec3();
        return true;
    }

    public bool GetMove
    {
        get { return move; }
        set { move = value; }
    }
}