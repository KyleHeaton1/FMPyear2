using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levels : MonoBehaviour
{
    public void Level1()
    {
        Debug.Log("LEVEL 1");
        SceneManager.LoadScene(1);
    }
}
