﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public float damage;
    public float speed;
    public float range;

    protected bool attacking = false;
    protected float timeToAttack = 1;     // brojac koji se smanjuje u zavisnosti od brzine oruzja, kada dodje do 0 ispali se projektil i vraca se brojac na 1
    protected Transform target;
                                                                                                                                                //DZO JE SERONJA!!! 09.18.2018. Djole :)
    virtual public void StartAttacking(Transform target)
    {
        if (!attacking)
        {
            attacking = true;
            timeToAttack = 1f;
            this.target = target;
        }
    }
    virtual public void Stop()
    {
        attacking = false;
    }

    virtual public void Update()
    {
        if (attacking)
        {
            timeToAttack -= speed * Time.deltaTime;

            if (timeToAttack <= 0)
            {
                Attack();
                timeToAttack = 1;
            }
        }
    }

    protected abstract void Attack();
}