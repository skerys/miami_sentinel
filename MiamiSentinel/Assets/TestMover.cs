using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMover : MonoBehaviour
{
    public float speed;
    public float stepFrequency;

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime * Mathf.Sqrt(Mathf.Cos(stepFrequency * Time.time + Mathf.PI) / 2f + .5f);
    }
}
