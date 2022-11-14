using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : CircuitOutput
{
    public override void ActivateAction(CircuitInput input)
    {
        ;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collide - {name}");
    }
}
