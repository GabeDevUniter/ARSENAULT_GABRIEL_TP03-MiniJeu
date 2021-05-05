using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BaseState
{
    private NPCMovement movement;

    public AlertState(Grunt grunt) : base(grunt)
    {
        movement = grunt.Movement;
    }

    public override void Start()
    {
        grunt.AudioRange.Trigger();

        movement.SetMode(NPCMovement.MoveMode.Alert);

        movement.SetMovement(NPCMovement.MovementTypes.Run);

        movement.StartMove(5f);

        grunt.Dialogs_Alert.Play();
    }

    public override void Tick()
    {
        if (Player.isDead) grunt._SetState(typeof(IdleState));

        grunt.FacePlayer(true);

        float range = Vector3.Distance(grunt.transform.position, grunt.PlayerHead) * 0.5f;

        if (grunt.Aim() && range <= grunt.currentWeapon.RangeNPC)
        {
            grunt._SetState(typeof(CombatState));
        }
    }

    public override void Stop()
    {
        movement.StopMove();
    }
}
