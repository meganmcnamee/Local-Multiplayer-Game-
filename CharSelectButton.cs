using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Megan McNamee

public class CharSelectButton : MonoBehaviour
{
    public SpriteRenderer theSR;

    public Sprite buttonUp, buttonDown;

    //when player jumps on button, it will be pressed. Then the button will pop up again to switch characters
    public bool isPressed;

    public float waitToPopUp;
    private float popCounter;

    public AnimatorOverrideController theController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPressed)
        {
            popCounter -= Time.deltaTime;

            if(popCounter <= 0)
            {
                isPressed = false;

                theSR.sprite = buttonUp;
            }
        }
    }

    //if player enters area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !isPressed)
        {
            PlayerController thePlayer = other.GetComponent<PlayerController>();

            if (thePlayer.theRB.velocity.y < -.1f)
            {
                thePlayer.anim.runtimeAnimatorController = theController;

                isPressed = true;

                theSR.sprite = buttonDown;

                popCounter = waitToPopUp;
            }

            AudioManager.instance.PlaySFX(2);
        }
    }

    //happens when player leaves area 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && isPressed)
        {
            isPressed = false;

            theSR.sprite = buttonUp;
        }
    }
}
