using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFoot : MonoBehaviour
{
    public int OnGroundTime;
    
    public bool OnGround => OnGroundTime > 0;
    public int LeaveGroundTime => OnGroundTime < 0 ? -OnGroundTime : 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (OnGroundTime > 0) OnGroundTime++;
        else OnGroundTime--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            OnGroundTime = 1;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            OnGroundTime = -1;
        }
    }
    
}
