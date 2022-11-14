using UnityEngine;


public class MTime
{
    public static float DeltaTime => Time.deltaTime * Application.targetFrameRate;
}

