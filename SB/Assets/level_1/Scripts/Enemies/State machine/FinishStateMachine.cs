using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishStateMachine 

{
    public State currentState { get; private set; }

    public void Initilialize(State startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
