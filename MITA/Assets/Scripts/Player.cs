using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float TimeJump;
    public float StartTimeJump;
    public float StartJumpCount;

    private bool IsGround = false;
    private float JumpCount = 1;
    private bool JumpControl;

    private Rigidbody2D rb;

    public GameObject GOisGround;
    public LayerMask Ground;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R)) // Для удобства(потом убрать надо)
        {
            gameObject.transform.position = new Vector2(0, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
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
        }

        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, Ground);
    }
}
