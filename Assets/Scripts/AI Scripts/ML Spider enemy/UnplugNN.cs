using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnplugNN : MonoBehaviour
{
    [SerializeField] private Health _spiderHealth;
    [SerializeField] private  CrawlerAgent _spiderBehaviour;
    [SerializeField] private GameObject _weapon;

    // Update is called once per frame
    void Update()
    {
        if(_spiderHealth._heathZero){Destroy(_spiderBehaviour); Destroy(_weapon);}
    }
}
