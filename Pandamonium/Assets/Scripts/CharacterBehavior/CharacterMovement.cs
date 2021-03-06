﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

// skripta za pracenje odgovarajuceg 'target' 3D objekta u 2D prostoru
public class CharacterMovement : MonoBehaviour {

    [HideInInspector]
    public Vector2 destination;

    private AIPath path;

    private void Start()
    {
        path = GetComponent<AIPath>();
    }

    public void MoveToPosition(Vector3 targetPosition) {


        path.isStopped = false;

        path.destination = targetPosition; // !!!
        path.SearchPath();
    }

    public void StopMoving()
    {
        path.isStopped = true;
    }
}
