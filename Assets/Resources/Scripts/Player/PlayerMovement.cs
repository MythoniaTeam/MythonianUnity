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
                //��ʼ��Ծ
                JumpEnd = false;
                //�ر���Ծ����
                JumpBufferTimeCount = 0;
                //����Ϊ��ͨˮƽ
                GravityBonusActivate = false;
                //���ٶ�
                Rigidbody.SetVel(y: JumpInitSpeed);
                //������Ծ����
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
    [SerializeField] private float GravityBonus; //���ɿ���Ծ�����������ӵ�ֵ

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
    /// <paramref name="vel"/> += <paramref name="acc"/>, �������� <paramref name="clampSpd2"/>��Χ��
    /// </summary>
    private float Accelerate(float vel, float acc, float? clamp = null)
    {
        return (clamp is float clampSpeed) 
            ?
            //��� vel ������� clamp ��Χ��, ֱ�ӷ��� vel
            (Mathf.Abs(vel) > clampSpeed) ? vel :
            //������ٺ󳬳� clamp ��Χ, ���� clamp
            (Mathf.Abs(vel + acc) > clampSpeed) ?
            Mathf.Sign(vel + acc) * clampSpeed :
            vel + acc 
            :
            vel + acc
            ;
    }

    /// <summary>���� <paramref name="accDirection"/> �ķ���, �� <paramref name="acc"/> ����, �������� <paramref name="clamp"/>��Χ��</summary>
    private float AccelerateByDir(float vel, float acc, float accDirection, float? clamp = null)
        => Accelerate(vel, acc * MathF.Sign(accDirection), clamp);

    /// <summary>���� <paramref name="vel"/> �ķ���, �� <paramref name="acc"/> ����, �������� <paramref name="clamp"/>��Χ��</summary>
    private float AccelerateByVelDir(float vel, float acc, float? clamp = null)
        => AccelerateByDir(vel, acc, clamp: clamp, accDirection: vel);


    /// <summary>
    /// <paramref name="vel"/> += <paramref name="dec"/>, �������� <paramref name="clampSpd2"/>��Χ��
    /// </summary>
    private float Decelerate(float vel, float dec, float? clamp = null)
    {
        return
            (clamp is float clampSpeed)
            ?
            //��� vel �� ��clamp ��Χ��, ֱ�ӷ��� vel
            (-clampSpeed <= vel && vel <= clampSpeed) ? vel :

            (vel > clampSpeed) ?
            //��� vel > clamp, ���ٺ� < clamp, ���� clamp, ���򷵻ؼ��ٺ���ٶ�
            (vel + dec < clampSpeed ? clampSpeed : vel + dec) :
            //��� vel < -clamp, ���ٺ� > -clamp, ���� -clamp, ���򷵻ؼ��ٺ���ٶ�
            (vel + dec > -clampSpeed ? -clampSpeed : vel + dec)
            :
            vel + dec;
    }

    /// <summary>���� <paramref name="acc"/> �ķ���, �� <paramref name="dec"/> ����, �������� <paramref name="clamp"/>��Χ��</summary>
    private float DecelerateByDirection(float vel, float acc, float accDirection, float? clamp = null)
        => Decelerate(vel, acc * MathF.Sign(accDirection), clamp);

    /// <summary>���� <paramref name="vel"/> �෴�ķ���, �� <paramref name="dec"/> ����, �������� <paramref name="clamp"/>��Χ��</summary>
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
            //��� ��ǰ�ٶ� > �����ٶ�����, ���ٶ��෴�������
            if (WalkSpeed > WalkSpeedMaxTemperary)
            {
                //�������ٶȷ�����ͬ, ���ٶȵ� ����ٶ�
                if (InputWalkStatus == WalkDirection)
                    WalkVelocity = DecelerateByVelDir(WalkVelocity, WalkDeceleration, clamp: WalkSpeedMaxTemperary);
                //�������ٶȷ����෴, �� ���ٶ� �� ���ٶ� �нϴ��߼���, 
                else
                    WalkVelocity = AccelerateByDir(WalkVelocity, acc: Mathf.Max(WalkDeceleration, walkAcc), accDirection: InputWalkStatus);
            }
            else
            {
                //�����հ�������ֱ�������ٶ�����, 
                WalkVelocity = AccelerateByDir(WalkVelocity, acc: walkAcc, accDirection: InputWalkStatus, clamp: WalkSpeedMaxTemperary);
            }
        }
    }

    #endregion WALK
}
