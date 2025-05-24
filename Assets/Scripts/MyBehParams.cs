using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBehParams : MonoBehaviour
{
    [SerializeField] public QLearning brain;
    [SerializeField] public int numActions = 4;
    public void UpdateQValues(string currentState, int actionTaken, float reward, string nextState)
    {
        brain.Train(currentState, actionTaken, reward, nextState);
    }
    public int DecideAction(string state)
    {
        int actions = brain.Decide(state);
        return actions;
    }
}
