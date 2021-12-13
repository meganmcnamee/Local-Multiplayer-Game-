﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// Megan McNamee

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed, jumpForce;
    // Start is called before the first frame update

    private float velocity;

    //keep track when player is on the ground, and drawing an invisible circle 
    private bool isGrounded;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;
    public Animator anim;
    public bool isKeyboard2;

    //Stop player from moving when using an attack
    public float timeBetweenAttacks = .25f;
    private float attackCounter;

    [HideInInspector]
    public float powerUpCounter;
    private float speedStore, gravStore;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        GameManager.instance.AddPlayer(this);

        speedStore = moveSpeed;
        gravStore = theRB.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(isKeyboard2)
        {
            velocity = 0f;

            if(Keyboard.current.lKey.isPressed)
            {
                velocity += 1f;
            }

            if(Keyboard.current.jKey.isPressed)
            {
                velocity = -1f;
            }

            if(isGrounded && Keyboard.current.rightShiftKey.wasPressedThisFrame)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            }

            if(!isGrounded && Keyboard.current.rightShiftKey.wasReleasedThisFrame && theRB.velocity.y > 0)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * .5f);
            }

            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                anim.SetTrigger("attack");

                attackCounter = timeBetweenAttacks;

                AudioManager.instance.PlaySFX(0);
            }
        }
        //will draw an imgainary circle in the world, check to draw the circle from, the radius of circle and what layer
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, whatIsGround);

        //make the player move around
        theRB.velocity = new Vector2(velocity * moveSpeed, theRB.velocity.y);

       
        if (Time.timeScale != 0)
        {
            anim.SetBool("isGrounded", isGrounded);

            anim.SetFloat("ySpeed", theRB.velocity.y);

            anim.SetFloat("speed", Mathf.Abs(theRB.velocity.x));

            if (theRB.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (theRB.velocity.x > 0)
            {
                transform.localScale = Vector3.one;
            }
        }

        if(attackCounter > 0)
        {
            //how long it takes for update to occur 
            attackCounter = attackCounter - Time.deltaTime;

            theRB.velocity = new Vector2(0f, theRB.velocity.y);
        }

        if(powerUpCounter > 0)
        {
            powerUpCounter -= Time.deltaTime;

            if(powerUpCounter <= 0)
            {
                moveSpeed = speedStore;
                theRB.gravityScale = gravStore;
            }
        }
    }

    //input system
    public void Move(InputAction.CallbackContext context)
    {
        velocity = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //release jump only when pressed button/key once
        if (context.started && isGrounded)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        }
        //only happen when moving upwards
        if (!isGrounded && context.canceled && theRB.velocity.y > 0f)
        {
            //if player is flying up in the air, go at half speed
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * .5f);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            anim.SetTrigger("attack");

            attackCounter = timeBetweenAttacks;

            AudioManager.instance.PlaySFX(0);
        }
    }
}
