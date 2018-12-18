﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomHolder : MonoBehaviour {

    public Tilemap groundTilemap;
    public Tilemap corridorTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap detailTilemap;
    public Tilemap foregroundTilemap;

    public Transform topEdge;
    public Transform bottomEdge;
    public Transform leftEdge;
    public Transform rightEdge;

    public Transform[] spawnPoints;

    private Tilemap acidTilemap;

    private Room context;

    public Vector3Int positionEndOfCorridor;

    private List<Vector2> stairsPositions;

    //public void Init(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    public void Init(Room context)
    {
        this.context = context;
        acidTilemap = LevelGeneration.I.acidTilemap;

        
    }

    private void Start()
    {
        stairsPositions = new List<Vector2>();
        DrawCorridors(context.doorTop, context.doorBot, context.doorLeft, context.doorRight);

        for (int i = Mathf.FloorToInt(transform.position.x - LevelGeneration.I.roomWidth / 2); i <= transform.position.x + LevelGeneration.I.roomWidth / 2; i++)
        {
            for (int j = Mathf.FloorToInt(transform.position.y - LevelGeneration.I.roomHeight / 2); j <= transform.position.y + LevelGeneration.I.roomHeight / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);

                Vector3Int groundTilePos = groundTilemap.WorldToCell(tilePos);

                if (groundTilemap.HasTile(groundTilePos))
                {
                    if (context.IsTileWalkable(groundTilemap, tilePos))// || context.IsTileWalkable(corridorTilemap, tilePos))
                    {
                        acidTilemap.SetTile(tilePos, null);
                        obstacleTilemap.SetTile(obstacleTilemap.WorldToCell(tilePos), null);
                    }
                    else if(!context.IsTileWalkable(corridorTilemap, tilePos) && !context.IsTileWalkable(LevelGeneration.I.corridorTilemap, tilePos))
                    {
                        bool walkable = false;
                        foreach(Vector2 stairsPos in stairsPositions)
                        {
                            if(tilePos.Equals(new Vector3Int(Mathf.FloorToInt(stairsPos.x), Mathf.FloorToInt(stairsPos.y) + 1, 0)))
                            {
                                walkable = true;
                                break;
                            }
                        }

                        if(!walkable)
                            obstacleTilemap.SetTile(obstacleTilemap.WorldToCell(tilePos), LevelGeneration.I.acidPrefab);
                    }

                    /*if (context.HasWalkableNeighbor(ref groundTilemap, tilePos))// || context.HasWalkableNeighbor(corridorTilemap, tilePos))
                    {
                        obstacleTilemap.SetTile(obstacleTilemap.WorldToCell(tilePos), LevelGeneration.I.acidPrefab);
                    }*/
                }
            }
        }

        
    }

    public Vector2 GetFirstSpawnPoint()
    {
        if (spawnPoints.Length > 0)
            return spawnPoints[0].position;
        else
            return Vector2.zero;
    }

    private void DrawCorridor(Vector2 start, Vector2 direction)
    {

        // napraviti stepenice

        Vector2 stairsPos = Vector2.zero;

        if(direction.Equals(Vector2.right) || direction.Equals(Vector2.left))
        {

            int dir = 1;

            float stairsAngle = 0;
            if (direction.Equals(Vector2.right))
            {
                stairsAngle = 0;
                dir = 1;
            }
            else if (direction.Equals(Vector2.left))
            {
                stairsAngle = 180;
                dir = -1;
            }

            GameObject stairs = Instantiate(LevelGeneration.I.stairsPrefab, start, Quaternion.Euler(0, stairsAngle, 0), transform);
            stairs.GetComponentInChildren<Stairs>().right = (dir == 1);
            stairsPos = start;
            stairsPositions.Add(stairsPos);
            //start.x += dir;
            start.y += 1;
            Vector3Int pos = obstacleTilemap.WorldToCell(start);
            obstacleTilemap.SetTile(pos, null);
        }

        Vector3Int tilePos = new Vector3Int();

        int i = 0;

        while (Mathf.Abs(start.x - transform.position.x) <= LevelGeneration.I.roomWidth / 2 &&
                Mathf.Abs(start.y - transform.position.y) <= LevelGeneration.I.roomHeight / 2)
        {
            tilePos = groundTilemap.WorldToCell(start);

            TileBase prefab = ((direction.Equals(Vector2.left) || direction.Equals(Vector2.right)) ? LevelGeneration.I.corridorHorizPrefab : LevelGeneration.I.corridorVertPrefab);

            corridorTilemap.SetTile(tilePos, prefab);

            if(direction.Equals(Vector2.left) || direction.Equals(Vector2.right))      // corridor left / right
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(0, 1, 0), prefab);
                corridorTilemap.SetTile(tilePos + new Vector3Int(0, -1, 0), prefab);

                if (!tilePos.Equals(stairsPos) && !context.IsTileWalkable(context.groundTilemap, tilePos + new Vector3Int(0, 1, 0)))
                {
                    obstacleTilemap.SetTile(tilePos + new Vector3Int(0, 1, 0), LevelGeneration.I.acidPrefab);
                }
                if (!context.IsTileWalkable(context.groundTilemap, tilePos + new Vector3Int(0, -1, 0)))
                {
                    obstacleTilemap.SetTile(tilePos + new Vector3Int(0, -1, 0), LevelGeneration.I.acidPrefab);
                }

                // foreground

                if(i == 0)
                {
                    if(direction.Equals(Vector2.left))
                        foregroundTilemap.SetTile(tilePos, LevelGeneration.I.corridorHorizForegroundLeftPrefab);
                    else
                        foregroundTilemap.SetTile(tilePos, LevelGeneration.I.corridorHorizForegroundRightPrefab);
                }
                else if(i > 0)
                    foregroundTilemap.SetTile(tilePos, LevelGeneration.I.corridorHorizForegroundPrefab);

            }
            else
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(1, 0, 0), prefab);
                corridorTilemap.SetTile(tilePos + new Vector3Int(-1, 0, 0), prefab);

                if (!context.IsTileWalkable(context.groundTilemap, tilePos + new Vector3Int(1, 0, 0)))
                {
                    obstacleTilemap.SetTile(tilePos + new Vector3Int(1, 0, 0), LevelGeneration.I.acidPrefab);
                }
                if (!context.IsTileWalkable(context.groundTilemap, tilePos + new Vector3Int(-1, 0, 0)))
                {
                    obstacleTilemap.SetTile(tilePos + new Vector3Int(-1, 0, 0), LevelGeneration.I.acidPrefab);
                }
            }

            acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);
            obstacleTilemap.SetTile(tilePos, null);

            start += direction;
            i++;
        }

        positionEndOfCorridor = tilePos;

        start -= direction;

        acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);

        Vector3Int corridorPos = LevelGeneration.I.corridorTilemap.WorldToCell(start);

        if (direction.Equals(Vector2.up) || direction.Equals(Vector2.down))
        {

            LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgeVertPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(1, 0, 0), LevelGeneration.I.corridorBridgeVertPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(-1, 0, 0), LevelGeneration.I.corridorBridgeVertPrefab);

        }
        else
        {
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgeHorizPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, 1, 0), LevelGeneration.I.corridorBridgeHorizPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, -1, 0), LevelGeneration.I.corridorBridgeHorizPrefab);

        }

    }

    public void DrawCorridors(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        if (doorTop)
        {
            DrawCorridor((Vector2)topEdge.position + new Vector2(0, -0.5f), Vector2.up); // proveriti za - 0.5f
        }
        if (doorBot)
        {
            DrawCorridor((Vector2)bottomEdge.position + new Vector2(0, 0.5f), Vector2.down); // proveriti za + 0.5f

        }
        if (doorLeft)
        {
            DrawCorridor((Vector2)leftEdge.position + new Vector2(0.5f, 0), Vector2.left); // proveriti za + 0.5f

        }
        if (doorRight)
        {
            DrawCorridor((Vector2)rightEdge.position + new Vector2(-0.5f, 0), Vector2.right); // proveriti za - 0.5f

        }

    }
}
