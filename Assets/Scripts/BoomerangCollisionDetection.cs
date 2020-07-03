using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangCollisionDetection : MonoBehaviour
{
    [SerializeField] Boomerang parentBoomerang;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        parentBoomerang.ProcessCollision(collision);
    }
}
