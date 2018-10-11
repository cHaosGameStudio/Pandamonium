﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{

    private Transform weapon;
    private Transform target;
    private float speed;

    private float damage;

    private bool shot = false;
    private Vector3 direction;

    public bool homing = false;

    public void Shoot(Transform weapon, Transform target, float speed)
    {
        this.weapon = weapon;
        this.target = target;
        direction = (target.position - transform.position).normalized;
        this.speed = speed;
        this.damage = weapon.GetComponent<Weapon>().damage;
        shot = true;

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);
    }

    private void Update()
    {
        if (!shot) return;

        if(homing && (target == null || weapon == null))      // ako je target unisten/ubijen u medjuvremenu
        {
            homing = false;
            return;
        }

        if (homing)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
            transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (weapon != null && other.gameObject == weapon.parent.gameObject)                   // ako je projektil pogodio pucaca
            return;

        AttackingCharacter character = other.GetComponent<AttackingCharacter>();

        if (other.transform == target && character != null)                                      // ako je pogodio metu
        {
            character.TakeDamage(damage, (other.transform.position - transform.position).normalized);
            shot = false;
            Destroy(gameObject);

            return;
        }

        if (character != null && weapon != null && character.type == weapon.parent.GetComponent<AttackingCharacter>().type)  // ako je pogodio karaktera istog tipa
        {
            return;
        }

        /*if(other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }*/

    }
}