using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float speed;
    public float movetime;

    public float goneTime;
    public bool isMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            goneTime += Time.deltaTime;

            if(goneTime >= movetime)
            {
                isMoving = false;

            }

        }

    }
}
