using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState
{

    protected Grunt grunt;

    public BaseState(Grunt grunt)
    {
        this.grunt = grunt;
    }

    public abstract void Start();

    public abstract void Tick();

    public abstract void Stop();
}
