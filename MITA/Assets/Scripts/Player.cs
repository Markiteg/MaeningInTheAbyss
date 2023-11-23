using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float TimeJump;
    public float StartTimeJump;
    public float StartJumpCount;
    public float TimeBTWDash;
    public float StartTimeBTWDash;
    public float TimeAFTDash;
    public float StartTimeAFTDash;
    public float DashSpeed;
    public float TimeWallJump;
    public float StartTimeWallJump;
    public float upDownSpeed;
    public float SlideSpeed;
    public float JumpWallTime;
    public Vector2 jumpAngle = new Vector2(3.5f, 10);
    public float FixYSpeed;

    private bool IsGround = false;
    private float JumpCount = 1;
    private bool JumpControl;
    private bool IsSide;
    private float DashCount = 1;
    private float StartDashCount = 1;
    private bool IsSite;
    private bool IsCanStay;
    private bool IsWallLeft, IsWallRight;
    private bool IsWall;
    private float GravityDev;
    private bool BlockMove = false;
    private float TimerJumpWall;
    private bool IsWallJumping = false;
    private bool IsDash = false;
    

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public GameObject GOisGround;
    public GameObject GOIsCanStay;
    public GameObject GOIsWallLeft;
    public GameObject GOIsWallRight;
    public LayerMask Ground;
    private void Awake()
    {
        IsSide = true;
        IsSite = false;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        GravityDev = rb.gravityScale;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R)) // Для удобства(потом убрать надо)
        {
            gameObject.transform.position = new Vector2(0, 0);
        }

        if (Input.GetKey(KeyCode.A)) // Лево
        {
            if (!BlockMove)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                IsSide = true;
            }
        }
        else if (Input.GetKey(KeyCode.D)) // Право
        {
            if (!BlockMove)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                IsSide = false;
            }
        }

        if (!BlockMove)
        {
            if (Input.GetKey(KeyCode.Space)) // Проверка на задержку пробела
            {
                if (IsGround)
                    JumpControl = true;
            }
            else
                JumpControl = false;
            if (JumpControl)
            {
                if (TimeJump > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    TimeJump -= Time.deltaTime;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !JumpControl && !IsGround && JumpCount > 0)
            {
                JumpCount--;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce + 1);
            }
            else if (IsGround)
            {
                TimeJump = StartTimeJump;
                JumpCount = StartJumpCount;
                DashCount = StartDashCount;
            }
        }

        if (IsWallRight && !IsGround || IsWallLeft && !IsGround) //Проверка на стене игрок или нет
        {
            JumpCount = StartJumpCount;
            DashCount = StartDashCount;
            IsWall = true;
        }
        else
            IsWall = false;

        if (IsWall && !IsGround) //Скольжение игрока
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, SlideSpeed);

            if (Input.GetKey(KeyCode.W))
            {
                rb.velocity = new Vector2(rb.velocity.x, upDownSpeed);
            }
        }
        else if (!IsGround && !IsWall)
            rb.gravityScale = GravityDev;

        if (IsWall && !IsGround && Input.GetKey(KeyCode.Space))//Прыжок от стены
        {
            BlockMove = true;
            TimerJumpWall = JumpWallTime;
            IsWallJumping = true;
        }
        if (BlockMove && TimerJumpWall <= 0)
        {
            BlockMove = false;
            IsWallJumping = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
            TimerJumpWall -= Time.deltaTime;
        if (IsWallJumping && IsWallLeft)
        {
            rb.velocity = new Vector2(jumpAngle.x, jumpAngle.y);
        }
        else if (IsWallJumping && IsWallRight)
        {
            rb.velocity = new Vector2(jumpAngle.x * -1, jumpAngle.y);
        }

        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, Ground); // Проверка на земле ли игрок
        IsCanStay = Physics2D.OverlapCircle(GOIsCanStay.transform.position, 0.5f, Ground); //Проверка может ли игрок выйти из режима приседа
        IsWallLeft = Physics2D.OverlapCircle(GOIsWallLeft.transform.position, 0.1f, Ground); //Проверка есть ли стена слева
        IsWallRight = Physics2D.OverlapCircle(GOIsWallRight.transform.position, 0.1f, Ground); // Проверка есть ли стена справа

        if (Input.GetKeyDown(KeyCode.Q) && TimeBTWDash <= 0) //Активация Дэша
            Dash();
        else
        {
            TimeBTWDash -= Time.deltaTime;
            if (!BlockMove)
            {
                if (TimeAFTDash <= 0 && IsDash)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    IsDash = false;
                }
                else
                    TimeAFTDash -= Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) //Приседания
        {
            if (!IsSite)
            {
                IsSite = true;
                bc.offset = new Vector2(0, -0.151821f);
                bc.size = new Vector2(1, 1.103642f);
            }
            else if (!IsCanStay)
            {
                IsSite = false;
                bc.offset = new Vector2(0, 0.3f);
                bc.size = new Vector2(1, 2);
            }
        }

        if (rb.velocity.y < FixYSpeed) //Ограничения скорости падения
            rb.velocity = new Vector2(rb.velocity.x, FixYSpeed);

        if (Input.GetKey(KeyCode.F) && !IsGround && !IsWallLeft && !IsWallRight && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -2);
        }
    }

    public void Dash() //Дэш
    {
        if (!IsSide) //Дэш в правую сторону
        {
            if (!IsGround && DashCount > 0) //Если дэш в воздухе
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                IsDash = true;
                DashCount--;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround) //Если дэш на земле
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            
        }
        else //Дэш в левую сторону
        {
            if (!IsGround && DashCount > 0) //Если дэш в воздухе
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                DashCount--;
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround) //Если дэш на земле
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
        }
    }
}
