using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //what to follow
    //public float smoothing = 5f; //camera speed

    //Vector3 offset;

    //void Start()
    //{
    //    offset = transform.position - target.position;
    //    //transform.position = new Vector3(target.position.x,
    //    //    target.position.y, transform.position.z);
    //}

    void Update()
    {
        if (!target) return;
        Vector3 targetCamPos = target.position;
        targetCamPos.y = transform.position.y;
        transform.position = targetCamPos;
        //transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
