using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate_Plate : MonoBehaviour
{
    public PressurePlate PressurePlate;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.y < 0 && IsCollidable(collision.collider))
        {
            PressurePlate.Activate();
        }
    }

    private bool IsCollidable(Collider2D collObj)
    {
        return true;
    }
}
