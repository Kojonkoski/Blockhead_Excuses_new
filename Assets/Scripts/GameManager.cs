using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class GameManager : MonoBehaviour
{
    public float transitionTime = 1f;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void Mainmenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EndGame()
    {
        StartCoroutine(LoadLevel(2));

    }

    public IEnumerator LoadLevel(int levelToLoad)
    {
        Time.timeScale = 1f;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelToLoad);
    }

    
}
