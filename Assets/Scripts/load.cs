using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class load : MonoBehaviour
{
    public Animator anim;
    public float time;
    string _scene;
    // Start is called before the first frame update

    public void Begin(string _sceneName)
    {
        _scene = _sceneName;
        StartCoroutine(WaitForEnd());
    }

    
    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(1);
        anim.SetBool("fade", true);
        yield return new WaitForSeconds(time);
        //if(loadCurernt) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FindObjectOfType<AudioManager>().StopAll();
        SceneManager.LoadScene(_scene);
    }
}
