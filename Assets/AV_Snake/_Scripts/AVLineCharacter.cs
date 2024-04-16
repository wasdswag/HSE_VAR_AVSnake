using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(LineRenderer))]
public class AVLineCharacter : MonoBehaviour
{

    [SerializeField, Range(0, 7)] private int freqFilterMin;
    [SerializeField, Range(0, 7)] private int freqFilterMax;
    
    [SerializeField] private Color highColor;
    [SerializeField] private Color lowColor;

    [SerializeField] private Color endhighColor;
    [SerializeField] private Color endlowColor;
    
    [SerializeField] private AudioData audioData;
    [SerializeField] private AnimationCurve avCurve;
    [SerializeField] private AnimationCurve formCurve;
    
    [SerializeField] private int numVertex = 20;
    [SerializeField] private float gap = 0.5f;

    [SerializeField] private float minWidth = 0.4f;
    [SerializeField] private float maxWidth = 30f;
    
    [SerializeField] private float minColor = 0.1f;
    [SerializeField] private float maxColor = 3f;
    
    [SerializeField] private float widthAmplifier = 2f;

    [SerializeField] private float idleMinDistance = 0.3f;
    [SerializeField] private float moveMinDistance = 0.2f;
    [SerializeField] private float segmentSpeed = 0.02f;
    [SerializeField] private float moveSpeed = 5f;

    private AVCharacterInput _avCharacterInput;
    private LineRenderer _lineRenderer;
    
    private Keyframe [] keys = new Keyframe[8];

    private Vector3[] segmentPositions;
    private Vector3[] segmentsVelocities;
    private Vector3 _followPosition;
    private Vector3 _direction;
    private float _distance;
    private float _distanteInterpolant;
    
    [SerializeField] private  AnimationCurve finalCurve;

    [SerializeField, Range(0f, 1f)] private float curvesInterpolation; 
   
    private void Start()
    {
        _avCharacterInput = GetComponent<AVCharacterInput>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = numVertex;
        segmentPositions = new Vector3[numVertex];
        segmentsVelocities = new Vector3[numVertex];
        
        SetWidthCurve();


        for (int i = 0; i < numVertex; i++)
        {
            var segmentPos = Vector3.right * i * gap;
            segmentPositions[i] = segmentPos;
        }
        
        _lineRenderer.SetPositions(segmentPositions);
        
    }

    float GetMinDistance(bool isMoved)
    {
        if (isMoved)
        {
            if (_distanteInterpolant < 1f)
                _distanteInterpolant += Time.deltaTime;
            else _distanteInterpolant = 1f;
        }
        else
        {
            if (_distanteInterpolant > 0f)
                _distanteInterpolant -= Time.deltaTime;
            else _distanteInterpolant = 0f;
        }

        return Mathf.Lerp(idleMinDistance, moveMinDistance, _distanteInterpolant);

    }
    
    private void Update()
    {
        bool isMoved = Input.GetMouseButton(0);
        _distance = GetMinDistance(isMoved);
        
        if (Input.GetMouseButton(0))
        {
            Vector3 desiredPosition = _avCharacterInput.GetCursorToWorldPosition();
            _direction = (segmentPositions[0]-desiredPosition).normalized;
            _followPosition = (desiredPosition - (_direction * moveMinDistance));
        }
        

        if (Vector3.Distance(segmentPositions[0], _followPosition) > moveMinDistance * 4)
        {
            segmentPositions[0] = Vector3.MoveTowards(segmentPositions[0],
                _followPosition, Time.deltaTime * moveSpeed);
        }

        for(int i = 1; i < segmentPositions.Length; i++)
        {
            segmentPositions[i] = Vector3.SmoothDamp(segmentPositions[i],
                segmentPositions[i - 1] + _direction * _distance,
                ref segmentsVelocities[i], segmentSpeed + (i * 0.001f));
        }
        
        _lineRenderer.SetPositions(segmentPositions);
        

        SetColor();
        UpdateWidthCurveByAudio();
        InterpolateCurves();
    }
    
    
    

    private void SetWidthCurve()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].time = i * (1f / avCurve.keys.Length);
            keys[i].value = 0.5f;
        }
        avCurve.keys = keys;
    }
    
    private void UpdateWidthCurveByAudio()
    {
        for (int n = freqFilterMin, i = 0; i < keys.Length; i++)
        {
            
            keys[i].value = minWidth + (GetAVSignal(n, minWidth, maxWidth) * widthAmplifier) / 2;
            n++;
            if (n > freqFilterMax) n = freqFilterMin;

        }

        avCurve.keys = keys;
    }

    private void InterpolateCurves()
    {
        Keyframe[] interpolated = new Keyframe[formCurve.keys.Length];
        for (int i = 0; i < interpolated.Length; i++)
        {
            var avKey = avCurve.keys[i];
            var formKey = formCurve.keys[i];

            interpolated[i].time = Mathf.Lerp(formKey.time, avKey.time, curvesInterpolation);
            interpolated[i].value = Mathf.Lerp(formKey.value, avKey.value, curvesInterpolation);
        }
        
        finalCurve.keys = interpolated;
        _lineRenderer.widthCurve = finalCurve;

    }


    private void SetColor()
    {
        var s = GetAVSignal(3, minColor, maxColor);
        var e = GetAVSignal(5, minColor, maxColor);

        _lineRenderer.startColor = Color.Lerp(lowColor, highColor, s);
        _lineRenderer.endColor = Color.Lerp(endlowColor, endhighColor, e);

    }

    private float GetAVSignal(int band, float min, float max)
    {
        var avSignal = audioData.GetBandFrequency(band);
        avSignal = Mathf.InverseLerp(min, max, avSignal);
        return avSignal;
    }
    
    
}
