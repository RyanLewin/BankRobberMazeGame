using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Pawn
{
    public Transform mobile;
    Weapon weapon;

    int drillsUsed = 0;
    public const int totalDrills = 3;
    string direction = "";
    bool move = false;

    void Update()
    {
        if (!gameManager.GameOverBool)
        {
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
            direction = "";

            if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
                weapon = (Weapon)0;
            else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
                weapon = (Weapon)1;
            else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
                weapon = (Weapon)2;

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
        otherPos = currCell.coordinates + dir.ToIntVec2();
        otherCell = cells[otherPos.x, otherPos.y];

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
        if (InRange())
        {
            foreach (Transform t in currCell.transform)
            {
                if (t.tag == "Wall")
                {
                    MapDirection wallDir = t.GetComponent<MapCellEdge>().dir;
                    if (dir == wallDir)
                    {
                        Instantiate(Resources.Load("Prefabs/Map_Wall_Frac"), t.position, t.rotation);
                        Destroy(t.gameObject);
                        drillsUsed++;
                        uiController.WeaponUI("Drills", totalDrills - drillsUsed);
                    }
                }
            }

            otherPos = currCell.coordinates + dir.ToIntVec2();
            otherCell = cells[otherPos.x, otherPos.y];

            foreach (Transform t in otherCell.transform)
            {
                if (t.tag == "Wall")
                {
                    MapDirection wallDir = t.GetComponent<MapCellEdge>().dir.GetOpposite();
                    if (dir == wallDir)
                    {
                        Destroy(t.gameObject);
                    }
                }
            }
        }
    }

    bool PlayerMove()
    {
        if (!Move())
            return false;

        gameManager.GetMoves++;
        uiController.Moves(gameManager.GetMoves);

        Camera.main.transform.position += dir.ToVec3();
        return true;
    }

    public bool GetMove
    {
        get { return move; }
        set { move = value; }
    }
}