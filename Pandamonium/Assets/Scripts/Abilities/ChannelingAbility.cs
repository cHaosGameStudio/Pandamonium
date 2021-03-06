﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelingAbility : Ability {

    public float tickInterval = 0.5f;

    private bool isChanneling = false;

    private float lastTick;

	public virtual void StartChanneling()
    {
        isChanneling = true;
        lastTick = Time.time;
    }

    public virtual void StopChanneling()
    {
        isChanneling = false;
    }

    public virtual void RotateChannel(Vector2 direction)
    {

    }

    protected override void Update()
    {
        if (isChanneling)
        {
            if(Time.time - lastTick > tickInterval)
            {
                DoTick();
            }
        }
    }

    protected virtual void DoTick()
    {
        if ((am.parent as PlayerWithJoystick).energy >= manaCost)
        {
            (am.parent as PlayerWithJoystick).DecreaseEnergy(manaCost);
            lastTick = Time.time;
        }
        else
        {
            StopChanneling();
        }
    }
}
