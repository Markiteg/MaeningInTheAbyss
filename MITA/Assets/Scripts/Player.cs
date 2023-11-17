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

    private bool IsGround = false;
    private float JumpCount = 1;
    private bool JumpControl;
    private bool IsSide;
    private float DashCount = 1;
    private float StartDashCount = 1;
    private bool IsSite;
    private bool IsCanStay;
    private bool IsWallLeft, IsWallRight;

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
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R)) // ��� ��������(����� ������ ����)
        {
            gameObject.transform.position = new Vector2(0, 0);
        }

        if (Input.GetKey(KeyCode.A)) // ����
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            IsSide = true;
        }
        else if (Input.GetKey(KeyCode.D)) // �����
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            IsSide = false;
        }

        if (Input.GetKey(KeyCode.Space)) // �������� �� �������� �������
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

        if (IsWallRight || IsWallLeft)
        {
            JumpCount = StartJumpCount;
            DashCount = StartDashCount;

            rb.velocity = new Vector2(rb.velocity.x, 0.4f);
        }

        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, Ground); // �������� �� ����� �� �����
        IsCanStay = Physics2D.OverlapCircle(GOIsCanStay.transform.position, 0.5f, Ground); //�������� ����� �� ����� ����� �� ������ �������
        IsWallLeft = Physics2D.OverlapCircle(GOIsWallLeft.transform.position, 0.1f, Ground); //�������� ���� �� ����� �����
        IsWallRight = Physics2D.OverlapCircle(GOIsWallRight.transform.position, 0.1f, Ground); // �������� ���� �� ����� ������

        if (Input.GetKeyDown(KeyCode.Q) && TimeBTWDash <= 0) //��������� ����
            Dash();
        else
        {
            TimeBTWDash -= Time.deltaTime;
            if (TimeAFTDash <= 0)
                rb.velocity = new Vector2(0, rb.velocity.y);
            else
                TimeAFTDash -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.C)) //����������
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
    }

    public void Dash() //���
    {
        if (!IsSide) //��� � ������ �������
        {
            if (!IsGround && DashCount > 0) //���� ��� � �������
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                DashCount--;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround) //���� ��� �� �����
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            
        }
        else //��� � ����� �������
        {
            if (!IsGround && DashCount > 0) //���� ��� � �������
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                DashCount--;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround) //���� ��� �� �����
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
        }
    }
}
