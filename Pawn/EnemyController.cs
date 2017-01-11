using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyController : Pawn {

    [Range(0f, 100f)]
    public float forwards = 40f, left = 25f, right = 25f, back = 10f; //Percentage chance of enemy turning or not
    int wait = 2; //if the enemy can see the player he will wait up to 2 turns before shooting

    void Update ()
    {
        //If player has moved then progress with enemy movement
        if (map.GetPlayer.GetMove)
            ChooseDir();
    }

    void LateUpdate ()
    {
        //If this is the last enemy, stop all movement
        if (this == map.GetEnemies.LastOrDefault())
            map.GetPlayer.GetMove = false;
    }

    void ChooseDir ()
    {
        //10 attempts to move before giving up and just facing the wall
        for (int i = 0; i < 10; i++)
        {
            //Random number to get the next direction
            float rand = Random.Range(0, 100);
            if (!stop)
            {
                //if the rand number is in lefts range
                if (rand > forwards && rand <= left + forwards)
                    if ((int)dir == 0)
                        dir = (MapDirection)3;
                    else
                        dir = (MapDirection)((int)dir - 1);
                //if the rand number is in rights range
                else if (rand > left + forwards && rand <= right + left + forwards)
                    if ((int)dir == 3)
                        dir = (MapDirection)0;
                    else
                        dir = (MapDirection)((int)dir + 1);
                //if the rand number is in backwards range
                else if (rand > right + left + forwards)
                    dir = dir.GetOpposite();

                //rotate enemy accordingly
                transform.rotation = dir.ToRotation();
            }

            //Check if the enemy can move
            if (Move())
            {
                //update currCell based on their direction
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
                break; //Break loop early if player is in view
            if (i == 9)
                return;
        }

        //Check if player can be seen from enemy's forward vector
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.ToVec3(), out hit, 15f))
        {
            //If player is visible, reduce wait int
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

        //if enemy is still stopped and wait time is over, end the game
        if (stop && wait == 0)
        {
            gameManager.GameOverBool = true;
            return;
        }
    }
}
