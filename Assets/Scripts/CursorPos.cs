using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPos : MonoBehaviour
{
    public static Vector2 position;

    void Update()
    {
        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
