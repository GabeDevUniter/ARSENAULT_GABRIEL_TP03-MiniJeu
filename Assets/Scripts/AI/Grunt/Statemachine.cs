using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statemachine : MonoBehaviour
{

    private Dictionary<System.Type, BaseState> states;

    protected BaseState CurrentState { get; private set; }

    protected void InitializeStates(Dictionary<System.Type, BaseState> states)
    {
        this.states = states;
    }

    protected void SetState(System.Type state)
    {
        if(CurrentState != null) CurrentState.Stop();

        CurrentState = states[state];

        //Debug.Log($"State set to: {CurrentState.GetType()}");

        CurrentState.Start();
    }

    protected void StateTick()
    {
        if (CurrentState != null) CurrentState.Tick();
    }
}
