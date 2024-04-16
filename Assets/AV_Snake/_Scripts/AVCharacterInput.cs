using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVCharacterInput : MonoBehaviour
{
    private Camera _camera;
    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }


    public Vector3 GetCursorToWorldPosition()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        Vector3 cursorWorldPos = _camera.ScreenToWorldPoint(mousePosition);
        return cursorWorldPos;
    }


}
