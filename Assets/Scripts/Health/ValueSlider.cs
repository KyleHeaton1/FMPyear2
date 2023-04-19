using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    public void SetMaxValue(int _value)
    {
        _slider.maxValue = _value;
        _slider.value = _value;
    }
    public void SetValue(int _value){_slider.value = _value;}
}
