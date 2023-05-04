using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsHeadline : MonoBehaviour
{
    public float _scrollspeed;
    float _log = 0;
    void Update()
    {
        _log += _scrollspeed;
        GetComponent<RawImage>().uvRect = new Rect(_log, 0, .07f, 1);
    }

}
