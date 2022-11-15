using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICircuitOutput
{
    public void ActivateAction(CircuitInput input) { }
    public void DeActivateAction(CircuitInput input) { }
}
