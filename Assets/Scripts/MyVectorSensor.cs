using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class MyVectorSensor : MonoBehaviour
{
    List<float> m_Observations;
    public MyVectorSensor(int size = 10)
    {
        m_Observations = new List<float>();
    }
    public void Clear()
    {
        m_Observations.Clear();
    }
    public void AddObservation(float observation)
    {
        m_Observations.Add(observation);
    }
    public void AddObservation(int observation)
    {
        AddObservation((float)observation);
    }
    public void AddObservation(Vector2 observation)
    {
        AddObservation(observation.x);
        AddObservation(observation.y);
    }
    public void AddObservation(Vector3 observation)
    {
        AddObservation(observation.x);
        AddObservation(observation.y);
        AddObservation(observation.z);
    }
    public List<float> GetObservations()
    {
        return m_Observations;
    }
}
