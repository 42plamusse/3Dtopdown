using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    [Space]
    [Header("Attributes:")]
    public float MOVEMENT_BASE_SPEED = 100.0f;
    public int DAMAGES = 5;

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            Vector3 nextStep = Vector3.forward * MOVEMENT_BASE_SPEED * Time.deltaTime;
            print(nextStep);
            Debug.DrawRay(transform.position, transform.forward * 1.0f, Color.green);
            if (Physics.Raycast(transform.position,
                transform.forward, out RaycastHit hit, 1.0f))
            {
                Collider other = hit.collider;
                print(other.name);
                if (other.CompareTag("Player"))
                    other.transform.parent.gameObject.GetComponent<Combat>().TakeDamage(DAMAGES);
                Destroy(gameObject);
            }
            transform.Translate(nextStep);
        }
    }
}
