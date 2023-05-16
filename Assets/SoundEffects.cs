using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    AudioManager _manager;
    public PlayerMovement _pm;
    void Start(){AudioManager _manager = FindObjectOfType<AudioManager>();}
    public void FootStep1(){if(_pm._grounded)FindObjectOfType<AudioManager>().PlayOneShotSound("step1");}
    public void FootStep2(){if(_pm._grounded)FindObjectOfType<AudioManager>().PlayOneShotSound("step2");}
}
