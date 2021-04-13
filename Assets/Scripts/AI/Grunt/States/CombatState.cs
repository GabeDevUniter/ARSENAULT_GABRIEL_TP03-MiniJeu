using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : BaseState
{

    public CombatState(Grunt grunt) : base(grunt)
    {

    }

    public override void Start()
    {
        
    }

    public override void Tick()
    {
        if (Player.isDead) grunt._SetState(typeof(IdleState));

        grunt.FacePlayer(false);

        if(grunt.IsFacingPlayer())
        {
            grunt.Shoot();

            if (!grunt.Aim())
            {
                grunt._SetState(typeof(AlertState));
            }
        }
    }

    public override void Stop()
    {
        
    }
}
