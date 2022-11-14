using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitInput : MonoBehaviour
{

    public bool IsActivated = false;
    private bool WasActivated = false;

    public List<CircuitOutput> outputs = new();

    // Update is called once per frame
    void Update()
    {
        if(IsActivated)
        {
            if(!WasActivated)
            foreach(CircuitOutput output in outputs)
            {
                output.ActivateAction(this);
            }
        }
        else
        {
            if(WasActivated)
            foreach (CircuitOutput output in outputs)
            {
                output.DeActivateAction(this);
            }
        }
        WasActivated = IsActivated;
    }
}
