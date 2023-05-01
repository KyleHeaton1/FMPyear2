using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsHeadline : MonoBehaviour
{
    public GameObject _textObj;
    public GameObject _headlineText;
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
        //_startingPos = _text.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _textObj.transform.Translate(-Vector3.right * _scrollSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(this.gameObject.tag == "box")
        {
            Invoke("ResetPos", 1);
            Debug.Log("fartsss");
        }
    }

    void ResetPos()
    {
        //_text.transform.position = _startingPos;
    }
}
