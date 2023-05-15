using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreFinder : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private ScoreSystem _scoreSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.text = "Score: " + _scoreSystem._score;
    }
}
