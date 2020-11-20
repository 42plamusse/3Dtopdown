using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float MOVE_BASE_SPEED = 6.0f;

    Rigidbody rb;
    Camera viewCamera;
    Vector3 velocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y,
            viewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
        velocity =
            new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"))
             * MOVE_BASE_SPEED;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
