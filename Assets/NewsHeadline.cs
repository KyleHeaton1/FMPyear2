using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsHeadline : MonoBehaviour
{
    public GameObject _text;
    public GameObject _breakingNewsObj;
    public TMP_Text _specificText;
    public string[] _headlines;
    public float _scrollSpeed;
    Vector3 _startingPos;
    int _index;
    // Start is called before the first frame update
    void Awake()
    {
        _specificText.text = _headlines[Random.Range(0, _headlines.Length)];
        _startingPos = _text.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _text.transform.Translate(-Vector3.right * _scrollSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2d(Collider2D other)
    {
        if(_breakingNewsObj.gameObject.tag == "box")
        {
            Invoke("ResetPos", 1);
            Debug.Log("fartsss");
        }
    }

    void ResetPos()
    {
        _text.transform.position = _startingPos;
    }
}
