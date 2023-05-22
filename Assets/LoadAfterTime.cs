using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAfterTime : MonoBehaviour
{   
    public load _load;
    void Start(){_load.Begin("MainMenu");}
}
