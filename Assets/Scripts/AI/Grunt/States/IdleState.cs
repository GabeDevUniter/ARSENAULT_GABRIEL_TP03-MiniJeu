using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    private NPCMovement movement;
    RaycastHit playerHit;

    public IdleState(Grunt grunt) : base(grunt)
    {
        movement = grunt.Movement;
    }

    public override void Start()
    {
        movement.SetMode(NPCMovement.MoveMode.Idle);

        movement.StartPatrol(NPCMovement.MovementTypes.Walk);
    }

    public override void Tick()
    {
        grunt.Dialogs_Idle.Play();

        if (!Player.isDead && grunt.PlayerDetect()) grunt._SetState(typeof(AlertState));
    }

    public override void Stop()
    {
        grunt.Dialogs_Idle.MuteAll();

        movement.StopPatrol();
    }
}
