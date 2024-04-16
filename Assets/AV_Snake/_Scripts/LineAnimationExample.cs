using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineAnimationExample : MonoBehaviour
{
    [SerializeField] private int numVertices = 10;
    [SerializeField] private float minVertexDistance = 1.2f;
    [SerializeField] private float radius = 2.5f;

    [SerializeField] private float minRadius = 0.8f;
    [SerializeField] private float maxRadius = 15f;


    [SerializeField] private Color highColor;
    [SerializeField] private Color lowColor;

    private AudioData _audioData;
    
    private LineRenderer _lineRenderer;
 
  

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = numVertices;
        _audioData = FindObjectOfType<AudioData>();
        SetLineColor(lowColor);

    }

    private void SetLineColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    // Update is called once per frame
    private void Update()
    {
        
        _lineRenderer.positionCount = Mathf.Clamp(numVertices, 2, 360);
        float angle = 0;
        int band = 0;


        
        
        for (int i = 0; i < numVertices; i++)
        {

            float r = radius + _audioData.GetBandFrequency(band);
            band++;
            if (band >= 7)
                band = 0;
            
            r = Mathf.Clamp(r, minRadius, maxRadius);
            float t = Mathf.InverseLerp(minRadius, maxRadius, r);

            Color lineColor = Color.Lerp(lowColor, highColor, t);
            SetLineColor(lineColor);
            
            
            float sin = Mathf.Sin(angle) * r;
            float cos = Mathf.Cos(angle) * r;
            
            
            Vector3 segmentPosition = Vector3.zero + new Vector3(sin, cos, 0);
            _lineRenderer.SetPosition(i, segmentPosition);
            
            
            angle += (2f * Mathf.PI) / numVertices;
        }

        
        
    }
}
