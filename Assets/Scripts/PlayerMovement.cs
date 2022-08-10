using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float rSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] float restartDelay = 2f;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animate;
    CapsuleCollider2D cc;
    BoxCollider2D ccFeet;
    bool isAlive = true;
    float gravityScaleAtStart;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animate = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();
        ccFeet = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb.gravityScale;
        
    }

    
    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Diee();
        DieSpike();
       
        
    }
    
    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }
    
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();


    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!ccFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }
    
    void Run()
    {
        if (!isAlive) { return; }

        Vector2 playerVelocity = new Vector2(moveInput.x * rSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;       
        bool playerHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animate.SetBool("isRunning", playerHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
            
        }
    }

    void ClimbLadder()
    {

        if (!ccFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = gravityScaleAtStart;
            animate.SetBool("isClimbing",false);
            return;
        }
        Vector2 climbVelocity = new Vector2 (rb.velocity.x, moveInput.y * climbSpeed);    
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;
        bool playerClimbingSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        animate.SetBool("isClimbing", playerClimbingSpeed);
        

    }
    void Diee()
    {
        if (cc.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {

            isAlive = false;
            animate.SetTrigger("Die");
            rb.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void DieSpike()
    {
        if (cc.IsTouchingLayers(LayerMask.GetMask("Spike")))
        {
            isAlive = false;
            animate.SetTrigger("Die");
            rb.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    
}
