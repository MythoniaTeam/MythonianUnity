using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitInput : MonoBehaviour, ICircuitOutput
{

    public void Activate() => IsActivated = true;
    public void DeActivate() => IsActivated = false;

    public bool IsActivated = false;
    private bool WasActivated = false;

    [SerializeField]
    public List<MonoBehaviour> Outputs = new();

    protected virtual void ActivateAction() { }
    protected virtual void DeActivateAction() { }
    void ICircuitOutput.ActivateAction(CircuitInput input) => ActivateAction();
    void ICircuitOutput.DeActivateAction(CircuitInput input) => DeActivateAction();

    private void Start()
    {
        if (!Outputs.Contains(this)) Outputs.Add(this);
    }


    // Update is called once per frame
    void Update()
    {
        if(IsActivated)
        {
            if(!WasActivated)
            foreach(ICircuitOutput output in Outputs)
            {
                output.ActivateAction(this);
            }
        }
        else
        {
            if(WasActivated)
            foreach (ICircuitOutput output in Outputs)
            {
                output.DeActivateAction(this);
            }
        }
        WasActivated = IsActivated;
    }
}
