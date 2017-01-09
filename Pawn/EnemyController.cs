using UnityEngine;
using System.Collections;

public class EnemyController : Pawn {

    [Range(0f, 100f)]
    public float forwards = 40f, left = 25f, right = 25f, back = 10f;
    int wait = 2;

    void Update ()
    {
        if (map.GetPlayer.GetMove)
        {
            ChooseDir();
            map.GetPlayer.GetMove = false;
        }
    }

    void ChooseDir ()
    {
        for (int i = 0; i < 10; i++)
        {
            float rand = Random.Range(0, 100);
            if (!stop)
            {
                if (rand > forwards && rand <= left + forwards)
                    if ((int)dir == 0)
                        dir = (MapDirection)3;
                    else
                        dir = (MapDirection)((int)dir - 1);
                else if (rand > left + forwards && rand <= right + left + forwards)
                    if ((int)dir == 3)
                        dir = (MapDirection)0;
                    else
                        dir = (MapDirection)((int)dir + 1);
                else if (rand > right + left + forwards)
                    dir = dir.GetOpposite();

                transform.rotation = dir.ToRotation();
            }

            if (Move())
            {
                switch (dir)
                {
                    case (MapDirection.North):
                        pos.y++;
                        currCell = cells[pos.x, pos.y];
                        break;
                    case (MapDirection.East):
                        pos.x++;
                        currCell = cells[pos.x, pos.y];
                        break;
                    case (MapDirection.South):
                        pos.y--;
                        currCell = cells[pos.x, pos.y];
                        break;
                    case (MapDirection.West):
                        pos.x--;
                        currCell = cells[pos.x, pos.y];
                        break;
                }
                break;
            }
            else if (stop)
                break;
            if (i == 9)
                return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.ToVec3(), out hit, 15f))
        {
            if (hit.transform.tag == "Player")
            {
                stop = true;
                wait--;
            }
            else
            {
                stop = false;
                if (wait != 2)
                    wait = 2;
            }
        }

        if (stop && wait == 0)
        {
            gameManager.GameOverBool = true;
            return;
        }
        
        if (currCell.transform.position.x != transform.position.x || currCell.transform.position.z != transform.position.z)
            Debug.Log("out of position " + currCell.transform.position + " " + transform.position);
    }
}
