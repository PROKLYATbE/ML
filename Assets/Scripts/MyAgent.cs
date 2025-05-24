using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.IO;

public struct MyAgentInfo
{
    public string prevState;
    public int prevAction;
}

public partial class MyAgent : MonoBehaviour
{
    public MyBehParams m_BehParams;
    public MyAgentInfo m_AgentInfo;
    private MyVectorSensor m_Sensors;
    private List<int> m_Actions;

    private float m_Reward;
    private float m_CumulateReward;
    private float m_LastRevard;
    private int m_StepCount = 17680000;

    string filePath = "default.txt";

    public virtual void Initialize() { }
    public virtual void CollectObservations(MyVectorSensor sensor) { }
    public virtual void OnActionReceived(List<int> actions) { }
    public virtual void OnEpisodeBegin() { }
    public virtual void Heuristic(in List<int> actionsOut) { }

    private void FixedUpdate()
    {
        CollectObservations(m_Sensors);
        string new_state = GetNextState(m_Sensors);

        if(!string.IsNullOrEmpty(m_AgentInfo.prevState) 
            && m_AgentInfo.prevAction != -1)
        {
            SendInfoToBrain(new_state);
        }
        
        int action = Decide();
        m_Actions[action] = 1;
        OnActionReceived(m_Actions);
        IncStep();

        if (m_StepCount % 10000 == 0)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                string formattedReward = m_CumulateReward.ToString("F5", System.Globalization.CultureInfo.InvariantCulture);
                writer.WriteLine($"{m_StepCount},{formattedReward}");
            }
        }

        m_AgentInfo.prevState = new_state;
        m_AgentInfo.prevAction = action;
        ResetData();
    }

    protected virtual void OnEnable()
    {
        m_BehParams = GetComponent<MyBehParams>();
        m_CumulateReward = -1127.02100f;
        m_Reward = 0;
        m_LastRevard = 0;

        m_AgentInfo = new MyAgentInfo();
        m_AgentInfo.prevState = "";
        m_AgentInfo.prevAction = -1;

        m_Actions = new List<int>(m_BehParams.numActions);
        m_Sensors = new MyVectorSensor();

        filePath = "MyQLearning.csv";
        using (StreamWriter writer = new StreamWriter(filePath, append: true))
        {
            //writer.WriteLine("StepCount,Reward");
        }
        _AgentReset();
        Initialize();
        Heuristic(m_Actions);
    }

    public void EndEpisode()
    {
        _AgentReset();
    }

    public string GetCurrentState()
    {
        return m_AgentInfo.prevState;
    }

    public int GetLastAction()
    {
        return m_AgentInfo.prevAction;
    }

    public string GetNextState(MyVectorSensor sensors)
    {
        List<float> lst = sensors.GetObservations();
        string res = "";
        res += lst[0].ToString() /*+ ":" + lst[1].ToString()*/ + ";";
        for (int i = 1; i < lst.Count; i++) 
        { 
            res += lst[i].ToString() + ";"; 
        }
            
        return res;
    }

    public void _AgentReset()
    {
        ResetData();
        m_Reward = 0;

        OnEpisodeBegin();
    }

    public void ResetData()
    {
        m_Sensors.Clear();
        for (int i = 0; i < m_Actions.Count; i++)
        {
            m_Actions[i] = 0;
        }
    }
    public int Decide()
    {
        string currentState = GetCurrentState();
        return m_BehParams.DecideAction(currentState);
    }

    public void IncStep()
    {
        ++m_StepCount;
    }

    public void SendInfoToBrain(string new_state)
    {
        m_BehParams.UpdateQValues(GetCurrentState(), GetLastAction(), m_LastRevard, new_state);
    }

    public void AddReward(float increment)
    {
        m_CumulateReward += increment;
        m_LastRevard = increment;
        m_Reward += increment;
    }
}
