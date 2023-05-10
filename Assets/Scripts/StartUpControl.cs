using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpControl : MonoBehaviour
{
    public float waitTime = 3;
    public GameObject mainScreen, FART;
    public Animator anim;
    bool isLoadedAlready;
    void Start()
    {
        StartCoroutine(WaitForTime());       
        anim = FART.GetComponent<Animator>();
    }
    private IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("Start", true);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
