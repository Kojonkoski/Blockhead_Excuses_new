using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;

public class questionScript : MonoBehaviour
{
    public GameObject question1;
    public GameObject answer1;
    public GameObject question2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine("questions");
        }

    }

    public IEnumerator questions()
    {
        Time.timeScale = 0;
        question1.SetActive(false);
        answer1.SetActive(true);
        yield break;
    }

    public void hideAnswers() 
    {
        answer1.SetActive(false);
        Time.timeScale = 1;
        question2.SetActive(true);
    }
}
