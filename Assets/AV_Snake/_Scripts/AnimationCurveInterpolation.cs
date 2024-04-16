using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveInterpolation : MonoBehaviour
{

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private AnimationCurve _curve;

    private Vector3 startPosition;
    private float t = 0;

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {

        if (t < 1.0)
            t += Time.deltaTime * 0.2f;

        var curveT = _curve.Evaluate(t);
        transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);


    }
}
