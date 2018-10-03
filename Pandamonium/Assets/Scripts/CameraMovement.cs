﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField]
    public Vector3 stageLowerLeft;
    public Vector3 stageUpperRight;

    private float minVisibleX;
    private float maxVisibleX;
    private float minVisibleZ;
    private float maxVisibleZ;

    public Transform player;

	// Use this for initialization
	void Start () {
        Vector3 LowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 UpperRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float viewportWidth = UpperRight.x - LowerLeft.x;
        float viewportHeight = UpperRight.z - LowerLeft.z;

        minVisibleX = stageLowerLeft.x + viewportWidth / 2f;
        maxVisibleX = stageUpperRight.x - viewportWidth / 2f;
        minVisibleZ = stageLowerLeft.z + viewportHeight / 2f;
        maxVisibleZ = stageUpperRight.z - viewportHeight / 2f;


        print(minVisibleX + ", " + maxVisibleX);
    }
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(Mathf.Clamp(player.position.x, minVisibleX, maxVisibleX), transform.position.y, Mathf.Clamp(player.position.z, minVisibleZ, maxVisibleZ));
	}
}