using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingController : MonoBehaviour
{
    private float speed = 0.5f;
    public float maxDist = 2f;
    private float walkedDist = 0;
    void Update()
    {
        if (walkedDist < maxDist)
        {
            this.gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            walkedDist += Vector3.forward.z * speed * Time.deltaTime;
        }
        else
        {
            this.gameObject.transform.Rotate(Vector3.up * 180, Space.Self);
            walkedDist = 0;
        }
    }
}
