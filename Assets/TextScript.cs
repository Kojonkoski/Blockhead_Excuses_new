using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public Camera lookAtThis;
    public Transform followThis;
    public GameObject text;
    public Vector3 offset;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {    
        transform.position = followThis.position+offset;
        transform.rotation = Quaternion.LookRotation(transform.position - lookAtThis.transform.position);
    }
}
