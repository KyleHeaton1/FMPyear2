using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timeFinder : MonoBehaviour
{
    public TMP_Text _text;
    int _hour;
    int _mins;

    // Update is called once per frame
    void Update()
    {
        _hour = System.DateTime.Now.Hour;
        _mins = System.DateTime.Now.Minute;
        _text.text = "" + _hour + ":" + _mins;    
    }
}
