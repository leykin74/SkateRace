using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_script : MonoBehaviour
{
    public Transform target;
    public float t;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
     void FixedUpdate()
    {
        transform.position=target.position;
        /*Vector3 a = transform.position;
        Vector3 b = target.position;      
        transform.position = Vector3.Lerp(a,b,t);*/
    }
}
