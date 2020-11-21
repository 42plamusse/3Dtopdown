using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Space]
    [Header("References:")]
    public Transform bulletSpawn;
    public Controller playerController;

    [Space]
    [Header("Prefabs:")]
    public GameObject bulletPrefab;

    [Space]
    [Header("Character attributes:")]
    public float RATE_OF_FIRE = 0.2f;
    public bool GUN_IS_AUTOMATIC;

    [Space]
    [Header("Character statistics:")]
    public float timeBeforeCanShoot = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (playerController.gunIsTriggered)
        {
            if (timeBeforeCanShoot <= 0.0f)
            {
                print("gun trigered");
                if (GUN_IS_AUTOMATIC ||
                    (!GUN_IS_AUTOMATIC && playerController.triggerReleased))
                {
                    playerController.Cmd_Shoot(bulletSpawn.position , bulletSpawn.rotation);
                }
                timeBeforeCanShoot = RATE_OF_FIRE;
            }
        }
        else
        {
            timeBeforeCanShoot = 0.0f;
        }
        timeBeforeCanShoot -= Time.deltaTime;
    }


}
