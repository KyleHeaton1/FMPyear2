using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnplugNN : MonoBehaviour
{
    [SerializeField] private Health _spiderHealth;
    [SerializeField] private GameObject _theSpider;
    [SerializeField] private  CrawlerAgent _spiderBehaviour;
    [SerializeField] private GameObject _weapon;
    void Update()
    {
        if(_spiderHealth._heathZero){Destroy(_spiderBehaviour); Destroy(_weapon);} 
        if(_spiderHealth._dead)Destroy(_theSpider);
    
    }
}
