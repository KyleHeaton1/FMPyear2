using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UItracker : MonoBehaviour
{
    public static UItracker instance;
    [HideInInspector] public bool _isOn;
    GameObject _newsUI;
    // Start is called before the first frame update
    void Start()
    {
        //makes it a singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        _newsUI = GameObject.Find("NewsHeadline");
        if(!_isOn) _newsUI.SetActive(false);
        else _newsUI.SetActive(true);
    }
}
