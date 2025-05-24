using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class QLearning : MonoBehaviour
{
    private Dictionary<string, float[]> qTable;
    private float learningRate;
    private float discountFactor;
    private float eps;
    private int numAction = 4;
    
    public bool isTrain = true;

    void Awake()
    {
        if (!isTrain)
        {
            QTableFromFile();
        }

        learningRate = 0.9f;
        discountFactor = 0.1f;
        eps = 0.1f;
        qTable = new Dictionary<string, float[]>();
    }

    private void OnDisable()
    {
        QTableToFile();
    }

    public int Decide(string state)
    {
        if (!qTable.ContainsKey(state))
        {
            qTable[state] = new float[numAction];
        }

        if (Random.value < eps)
        {
            return Random.Range(0, numAction);
        }
        else
        {
            int maxIndex = 0;
            for (int i = 1; i < numAction; i++)
            {
                if (qTable[state][i] > qTable[state][maxIndex])
                {
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
    }

    public void Train(string state, float action, float reward, string nextState)
    {
        if (!qTable.ContainsKey(state))
        {
            qTable[state] = new float[numAction];
        }

        if (!qTable.ContainsKey(nextState))
        {
            qTable[nextState] = new float[numAction];
        }

        float bestNextQ = float.MinValue;
        for (int i= 0; i < numAction; i++)
        {
            bestNextQ = Mathf.Max(bestNextQ, qTable[state][i]);
        }

        if (reward > 0)
        {
            reward = reward;
        }

        qTable[state][(int)action] += learningRate * (reward + discountFactor * bestNextQ - qTable[state][(int)action]);
    }

    public Dictionary<string, float[]> GetQTable()
    {
        return qTable;
    }
    public void QTableToFile()
    {
        string json = JsonConvert.SerializeObject(qTable, Formatting.Indented);
        File.WriteAllText("qTable.json", json);
    }
    public void QTableFromFile()
    {
        if (File.Exists("qTable.json"))
        {
            string json = File.ReadAllText("qTable.json");
            qTable = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(json);
        }
        else
        {
            qTable = new Dictionary<string, float[]>();
        }
    }
}
