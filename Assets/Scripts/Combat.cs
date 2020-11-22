using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Combat : NetworkBehaviour
{
    [SyncVar]
    public int health = 100;
    [SyncVar]
    public int nbrDeath = 0;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;
        health -= amount;
        if (health <= 0)
            Death();
    }

    void Death()
    {
        nbrDeath++;
        health = 100;
        Rcp_ChangeColor();
        //Trpc_respawn();
    }

    [ClientRpc]
    void Rcp_ChangeColor()
    {
        GetComponentInChildren<Renderer>().material.color = Color.red;
    }
    //[TargetRpc]
    //void Trpc_respawn()
    //{
    //    transform.position = GetComponent<Player>().initialPosition;

    //}
}
