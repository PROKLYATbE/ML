using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;
using System;
using static UnityEngine.GraphicsBuffer;

public class MyAgentInstance : MyAgent
{
    [SerializeField] Player player;
    [SerializeField] GameManager gm;

    private int count_inv = int.MaxValue;
    private int count_lives;
    private int count_lives_prev;

    public override void Initialize()
    {
        player = this.GetComponent<Player>();
        gm = GameObject.FindObjectOfType<GameManager>();
        count_lives = gm.lives;
        count_lives_prev = count_lives;
        GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
        count_inv = invaders.Length;
    }
    public override void Heuristic(in List<int> actionsOut)
    {
        actionsOut.Add(0); //Стоять ли
        actionsOut.Add(0); //Влево ли
        actionsOut.Add(0); //Вправо ли
        actionsOut.Add(0); //Стрелять ли
    }
    public override void OnActionReceived(List<int> actions)
    {
        if (actions[1] == 1)
        {
            player.MoveLeft();
        }
        else if (actions[2] == 1)
        {
            player.MoveRight();
        }
        else if (actions[3] == 1)
        {
            //AddReward(-0.01f);
            player.Shoot();
        }
        count_lives = gm.lives;

        AddReward(-0.001f);
        GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
        GameObject laser = GameObject.FindGameObjectWithTag("Laser");
        //count_lives = gm.lives;

        //if (count_lives < count_lives_prev)
        //{
        //    count_lives_prev = count_lives;
        //    AddReward(-0.1f);
        //}
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
    public override void CollectObservations(MyVectorSensor sensor)
    {
        sensor.AddObservation((float)Math.Round(this.transform.position.x, 1));
        //sensor.AddObservation((float)Math.Round(this.transform.position.y, 1));

        //GameObject missile = GameObject.FindGameObjectWithTag("Missile");
        //sensor.AddObservation(missile != null ? (this.transform.position - missile.transform.position).magnitude : -1);

        GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
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
        
        foreach (GameObject invader in invaders)
        {
            //if (l > (int)(this.transform.position - invader.transform.position).magnitude)
            //{
            //nearestInv = invader;
            //l = (int)(this.transform.position - nearestInv.transform.position).magnitude;
            //}
            
            sensor.AddObservation(invader != null ? (float)Math.Round((this.transform.position - invader.transform.position).magnitude, 1) : -1);
            //sensor.AddObservation(invader != null ? (float)Math.Round(invader.transform.position.x, 1) : -1);
            //sensor.AddObservation(invader != null ? (float)Math.Round(invader.transform.position.y, 1) : -1);
        }
        
    }
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
}
