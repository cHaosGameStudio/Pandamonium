﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogocidRangeAnimation : CharacterAnimation
{

    AttackingCharacter.PlayerState state;

    protected override void Start()
    {
        base.Start();
        state = transform.parent.GetComponent<Enemy>().playerState;
    }

    protected override void FlipAnimation()
    {
        updateVector2();

        angle = Vector2.Angle(vector2, new Vector2(1, 0));

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //if ((angle > 0 && angle < 90))

        if (vector2.x < 0)
            sr.flipX = true;
        else if (vector2.x > 0) sr.flipX = false;
    }

    void Update()
    {
        //animation360();

        FlipAnimation();


        AttackingCharacter.PlayerState currentState = transform.parent.GetComponent<Frogocite>().playerState;
        if (!currentState.Equals(state))
        {
            switch (state)
            {
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", false);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    animator.SetBool("Jumping", false);
                    break;
            }
            state = currentState;
            switch (state)
            {
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", true);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    if (transform.parent.GetComponent<Frogocite>().isJumping)
                    {
                        animator.SetBool("Jumping", true);
                    }
                    if (transform.parent.GetComponent<Frogocite>().isDead)
                    {
                        animator.SetBool("Dying", true);
                    }
                    break;

            }
        }

        /*
         * case AttackingCharacter.PlayerState.IDLE:
                    animator.SetBool("Walking", false);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", true);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Walking", false);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    if (transform.parent.GetComponent<Frogocite>().isJumping)
                    {
                        animator.SetBool("Walking", false);
                        animator.SetBool("Attacking", false);
                        animator.SetBool("Jumping", true);
                    }
                    if (transform.parent.GetComponent<Frogocite>().isDead)
                    {
                        animator.SetBool("Walking", false);
                        animator.SetBool("Jumping", false);
                        animator.SetBool("Attacking", false);
                        animator.SetBool("Dying", true);
                    }
                    break;
                    */

    }
}
