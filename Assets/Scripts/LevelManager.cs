using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void Select(GameObject _current)
    {
        _current.GetComponent<Button>().Select();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }

    public void Click()
    {
        FindObjectOfType<AudioManager>().PlayOneShotSound("click");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
