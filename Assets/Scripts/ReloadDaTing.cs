using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadDaTing : MonoBehaviour
{
    void Update()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
    }
}
