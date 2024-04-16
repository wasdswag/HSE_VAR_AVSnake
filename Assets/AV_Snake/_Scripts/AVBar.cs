using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVBar : MonoBehaviour
{
    [SerializeField] private int band;
    [SerializeField] private AudioData audioData;

    [SerializeField] private float amplifier = 1f;
    private float max, min;
    
  
    void Update()
    {
        transform.localScale = Vector3.one + Vector3.up * audioData.GetBandFrequency(band) ;
  
    }
}
