using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour
{
    [Space]
    [Header("References:")]
    public Gun currentGun;

    [Space]
    [Header("Prefabs:")]

    [Space]
    [Header("Character attributes:")]
    public float MOVEMENT_BASE_SPEED = 6.0f;

    [Space]
    [Header("Character statistics:")]
    public Vector3 velocity;
    public Vector3 mousePos;



    private Rigidbody rb;
    private Camera viewCamera;


    public bool gunIsTriggered;
    public bool triggerReleased;

    private void Start()
    {
        if (isLocalPlayer)
        {
            rb = GetComponent<Rigidbody>();
            viewCamera = Camera.main;
        }
    }


    private void ProcessInput()
    {
        mousePos = viewCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y,
            viewCamera.transform.position.y));
        velocity =
            new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))
             * MOVEMENT_BASE_SPEED;
        if (Input.GetMouseButtonDown(0)) triggerReleased = false;
        gunIsTriggered = Input.GetMouseButton(0);
        if (Input.GetMouseButtonUp(0)) triggerReleased = true;

    } 

    private void Update()
    {
        if (isLocalPlayer)
        {
            ProcessInput();
            LookAt();
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void LookAt()
    {
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
    }

    [Command]
    public void Cmd_Shoot(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject bullet = Instantiate(currentGun.bulletPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }
}
