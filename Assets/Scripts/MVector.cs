using UnityEngine;


//public class VectorExtension
//{
//    public void Add(this Vector2 v, float x = 0, float y = 0)
//    {
//        v.x += x;
//    }
//}

public static class Extension
{
    public static void AddVel(this Rigidbody2D rigidbody, float x = 0, float y = 0)
    {
        rigidbody.velocity += new Vector2(x, y);
    }
    public static void SetVel(this Rigidbody2D rigidbody, float? x = null, float? y = null)
    {
        rigidbody.velocity = new(x ?? rigidbody.velocity.x, y ?? rigidbody.velocity.y);
    }
}
