using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Linq;

public class MLAgentInstance : Agent
{
    Player player;
    GameManager gm;
    GameObject[] invaders;

    private int count_lives;
    private int count_lives_prev;
    private int count_inv = int.MaxValue;
    public override void Initialize()
    {
        player = this.GetComponent<Player>();
        gm = GameObject.FindObjectOfType<GameManager>();
        count_lives = gm.lives;
        count_lives_prev = count_lives;

        invaders = GameObject.FindGameObjectsWithTag("Invader");
        count_inv = invaders.Length;
    }
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var descrAction = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.A))
        {
            descrAction[0] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            descrAction[0] = 2;
        }
        else 
        { 
            descrAction[0] = 0;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            descrAction[1] = 1;
        }
        else
        {
            descrAction[1] = 0;
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 1)
        {
            player.MoveLeft();
        }
        else if (actions.DiscreteActions[0] == 2)
        {
            player.MoveRight();
        }
        if (actions.DiscreteActions[1] == 1)
        {
            player.Shoot();
        }
        AddReward(-0.01f);
        GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
        GameObject laser = GameObject.FindGameObjectWithTag("Laser");
        if (invaders.Length < count_inv)
        {
            count_inv = invaders.Length;
            AddReward(0.1f);
        }
        if (invaders.Count() == 1 && laser != null && (laser.transform.position - invaders[0].transform.position).magnitude < 1.0f)
        {
            AddReward(1f);
            EndEpisode();
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((float)Math.Round(this.transform.position.x, 1));
        //GameObject missile = GameObject.FindGameObjectWithTag("Missile");
        //sensor.AddObservation(missile != null ? (this.transform.position - missile.transform.position).magnitude : -1);

        if (invaders.Count() != 0)
        {
           var direct = (invaders[0].transform.position - transform.position).normalized;
           if (direct.y < 0)
           {
             sensor.AddObservation(-1); //Вправо
          }
          else if (direct.y > 0)
           {
               sensor.AddObservation(1); //Влево
          }
           else
           {
             sensor.AddObservation(0);
           }
        }
        invaders = GameObject.FindGameObjectsWithTag("Invader");
        for (int i = 0; i < invaders.Length; i++)
        {
            sensor.AddObservation(invaders[i] != null ? (float)Math.Round((this.transform.position - invaders[i].transform.position).magnitude, 1) : -1);
        }

    }
}
