using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JoystickBG"))
        {
            player.GetComponent<MovementController>().SetIsRunning(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JoystickBG"))
        {
            player.GetComponent<MovementController>().SetIsRunning(false);
        }
    }
}
