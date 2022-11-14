using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public PlayerFoot Foot;
    public Rigidbody2D Rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        JumpChance = JumpChanceMax;
    }

    
    void FixedUpdate()
    {

        GetInputStatus();
        OnGroundAction();
        CalculateApex();

        CalculateJump();

        CalculateGravity();

        CalculateWalk();

    }




    #region INPUT

    [Header("INPUT")]

    [SerializeField] private MInput Input;

    [SerializeField] private sbyte InputWalkStatus;
    //[SerializeField] private bool JumpKeyDownThisFrame, JumpKeyReleased;
    //[ReadOnly][SerializeField] private int JumpKeyPressTime = -1;
    [SerializeField] private bool InputJumpThisFrame;
    [ReadOnly][SerializeField] private bool GravityBonusActivate;

    private void GetInputStatus()
    {

        if (Input.KeyPressedThisFrame(MKeyName.Jump)) InputJumpThisFrame = true;

        else if (Input.KeyReleased(MKeyName.Jump)) GravityBonusActivate = true;

        InputWalkStatus = 0;
        if (Input.KeyPressed(MKeyName.Left)) InputWalkStatus--;
        if (Input.KeyPressed(MKeyName.Right)) InputWalkStatus++;
        
    }

    #endregion INPUT




    #region JUMP
    [Header("JUMP")]

    [ReadOnly]
    public bool a;
    public float JumpInitSpeed;

    [SerializeField] private int JumpChanceMax = 2;
    [SerializeField] private int JumpChance;
    [ReadOnly] [SerializeField] private bool JumpEnd;
    [ReadOnly] [SerializeField] private int JumpBufferTimeCount;
    [SerializeField] private int JumpBufferTimeMax;

    public void CalculateJump()
    {
        if (InputJumpThisFrame)
        {
            InputJumpThisFrame = false;

            JumpBufferTimeCount = JumpBufferTimeMax;
        }
        if (JumpBufferTimeCount > 0)
        {
            JumpBufferTimeCount--;
            if (JumpChance > 0)
            {
                //开始跳跃
                JumpEnd = false;
                //关闭跳跃缓冲
                JumpBufferTimeCount = 0;
                //重力为普通水平
                GravityBonusActivate = false;
                //加速度
                Rigidbody.SetVel(y: JumpInitSpeed);
                //减少跳跃次数
                JumpChance--;
                //Debug.LogWarning("Jump");
            }
        }

    }

    #endregion JUMP




    #region GRAVITY
    [Header("GRAVITY")]

    [SerializeField] private float Gravity;
    [SerializeField] private float FallSpeedMax;
    [SerializeField] private float GravityBonus; //当松开跳跃键后重力增加的值

    public void CalculateGravity()
    {
        if (GravityBonusActivate)
            ApplyGravity(Gravity + GravityBonus);
        else ApplyGravity(Gravity);
    }

    public void ApplyGravity(float gravityValue)
    {
        if(Rigidbody.velocity.y > -FallSpeedMax)
        {
            Rigidbody.AddVel(y: -gravityValue * Mathf.Lerp(1, 1 - ApexGravityReducion, ApexLevel));
            if (Rigidbody.velocity.y < -FallSpeedMax)
                Rigidbody.SetVel(y: -FallSpeedMax);
        }
    }

    public void OnGroundAction()
    {
        if (Foot.OnGround)
        {
            JumpChance = JumpChanceMax;
            JumpEnd = true;
        }
        if (JumpEnd && Foot.LeaveGroundTime == CoyoteTime)
        {
            //Debug.Log($"JumpChance = {JumpChance} - 1");
            JumpChance--;
        }

    }


    [SerializeField] private float ApexSpeedThreshold = 1;
    [ReadOnly]
    [SerializeField] private float ApexLevel = 1;
    [SerializeField] private float ApexGravityReducion = 0.8f;
    [SerializeField] private float ApexWalkAccelerationBonus = 0.5f;
    [SerializeField] private float ApexWalkSpeedBonus = 0.5f;
    [SerializeField] private int CoyoteTime = 20;

    private bool Apex = false;
    private void CalculateApex()
    {
        ApexLevel = JumpEnd ? 0 :
            Mathf.Max(0, Mathf.InverseLerp(ApexSpeedThreshold, 0, MathF.Abs(Rigidbody.velocity.y)));
        //if (ApexLevel > 0) { if (Apex == false) Debug.Log("StartApex"); Apex = true;  }
        //else               { if (Apex == true ) Debug.Log("EndApex");   Apex = false; }
    }

    #endregion GRAVITY




    #region WALK
    [Header("WALK")]

    [SerializeField] private float WalkAcceleration;
    [SerializeField] private float WalkDeceleration;
    [SerializeField] private float WalkSpeedMax;
    [ReadOnly][SerializeField] private float WalkSpeedMaxTemperary;
    public float WalkSpeed => MathF.Abs(Rigidbody.velocity.x);
    public float WalkVelocity
    {
        get => Rigidbody.velocity.x;
        set => Rigidbody.SetVel(x: value);
    } 
    public int WalkDirection => MathF.Sign(Rigidbody.velocity.x);


    #region Methods Accelerate

    /// <summary>
    /// <paramref name="vel"/> += <paramref name="acc"/>, 并限制在 <paramref name="clampSpd2"/>范围内
    /// </summary>
    private float Accelerate(float vel, float acc, float? clamp = null)
    {
        return (clamp is float clampSpeed) 
            ?
            //如果 vel 本身就在 clamp 范围外, 直接返回 vel
            (Mathf.Abs(vel) > clampSpeed) ? vel :
            //如果加速后超出 clamp 范围, 返回 clamp
            (Mathf.Abs(vel + acc) > clampSpeed) ?
            Mathf.Sign(vel + acc) * clampSpeed :
            vel + acc 
            :
            vel + acc
            ;
    }

    /// <summary>朝着 <paramref name="accDirection"/> 的方向, 以 <paramref name="acc"/> 加速, 并限制在 <paramref name="clamp"/>范围内</summary>
    private float AccelerateByDir(float vel, float acc, float accDirection, float? clamp = null)
        => Accelerate(vel, acc * MathF.Sign(accDirection), clamp);

    /// <summary>朝着 <paramref name="vel"/> 的方向, 以 <paramref name="acc"/> 加速, 并限制在 <paramref name="clamp"/>范围内</summary>
    private float AccelerateByVelDir(float vel, float acc, float? clamp = null)
        => AccelerateByDir(vel, acc, clamp: clamp, accDirection: vel);


    /// <summary>
    /// <paramref name="vel"/> += <paramref name="dec"/>, 并限制在 <paramref name="clampSpd2"/>范围外
    /// </summary>
    private float Decelerate(float vel, float dec, float? clamp = null)
    {
        return
            (clamp is float clampSpeed)
            ?
            //如果 vel 在 ±clamp 范围内, 直接返回 vel
            (-clampSpeed <= vel && vel <= clampSpeed) ? vel :

            (vel > clampSpeed) ?
            //如果 vel > clamp, 减速后 < clamp, 返回 clamp, 否则返回减速后的速度
            (vel + dec < clampSpeed ? clampSpeed : vel + dec) :
            //如果 vel < -clamp, 减速后 > -clamp, 返回 -clamp, 否则返回减速后的速度
            (vel + dec > -clampSpeed ? -clampSpeed : vel + dec)
            :
            vel + dec;
    }

    /// <summary>朝着 <paramref name="acc"/> 的方向, 以 <paramref name="dec"/> 减速, 并限制在 <paramref name="clamp"/>范围外</summary>
    private float DecelerateByDirection(float vel, float acc, float accDirection, float? clamp = null)
        => Decelerate(vel, acc * MathF.Sign(accDirection), clamp);

    /// <summary>朝着 <paramref name="vel"/> 相反的方向, 以 <paramref name="dec"/> 减速, 并限制在 <paramref name="clamp"/>范围外</summary>
    private float DecelerateByVelDir(float vel, float dec, float? clamp = null)
        => DecelerateByDirection(vel, dec, clamp: clamp, accDirection: -vel);
    
    

    #endregion Methods Accelerate

    private void CalculateWalk()
    {
        var walkAcc = WalkAcceleration * (1 + ApexLevel * ApexWalkAccelerationBonus);
        WalkSpeedMaxTemperary = WalkSpeedMax * (1 + ApexLevel * ApexWalkSpeedBonus);
        if (InputWalkStatus == 0) WalkVelocity = DecelerateByVelDir(WalkVelocity, WalkDeceleration, 0);
        else
        {
            //如果 当前速度 > 步行速度上限, 朝速度相反方向减速
            if (WalkSpeed > WalkSpeedMaxTemperary)
            {
                //按键和速度方向相同, 减速度到 最大速度
                if (InputWalkStatus == WalkDirection)
                    WalkVelocity = DecelerateByVelDir(WalkVelocity, WalkDeceleration, clamp: WalkSpeedMaxTemperary);
                //按键和速度方向相反, 以 减速度 和 加速度 中较大者减速, 
                else
                    WalkVelocity = AccelerateByDir(WalkVelocity, acc: Mathf.Max(WalkDeceleration, walkAcc), accDirection: InputWalkStatus);
            }
            else
            {
                //否则按照按键加速直到步行速度上限, 
                WalkVelocity = AccelerateByDir(WalkVelocity, acc: walkAcc, accDirection: InputWalkStatus, clamp: WalkSpeedMaxTemperary);
            }
        }
    }

    #endregion WALK
}
