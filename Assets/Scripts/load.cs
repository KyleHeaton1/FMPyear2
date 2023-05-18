using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class load : MonoBehaviour
{
    public Animator anim;
    public float time;
    string _scene;
    bool loadCurernt;
    // Start is called before the first frame update
    public void loadCurrentScene(bool loadCurernt)
    {
        if(loadCurernt == true) loadCurernt = true;
        else loadCurernt = false;
    }
    public void Begin(string _sceneName)
    {
        _scene = _sceneName;
        StartCoroutine(WaitForEnd());
    }

    
    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(time);
        anim.SetBool("fade", true);
        yield return new WaitForSeconds(1);
        if(loadCurernt) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else SceneManager.LoadScene(_scene);
    }
}
