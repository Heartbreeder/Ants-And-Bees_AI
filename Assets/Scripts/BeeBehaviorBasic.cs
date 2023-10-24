using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BeeBehaviorBasic : MonoBehaviour
{
    float thresholdqr = 10000f;
    //For how many steps a particular pheromone can be propagated
    int threshold_hopcount = 5;
    //This is the decay factor of the pheromone, must be set externally
    float hopcount_decay = 0.8f;
    //Indicates if agent is a queen or not
    bool QR = false;
    //Current level of pheromone of the agent
    float h = 1000f;
    //Current received pheromones
    public List<GameObject> received_pheromones = new List<GameObject>();
    //Initialized externaly
    //Current position of the agent
    int x = 0;
    int y = 0;

    //Last known destination of the agent
    float current_magnitude = 5f;
    float current_theta_dest = 0f;
    //How many steps till decay cycle executes
    int time_decay_steps = 3;
    int current_time_step = 0;
    float time_decay = 0.6f;
    int hopcount = 0;

    public bool test = true;
    public float test_pheromone = 0f;
    public int list_count;
    bool calc_round = true;

    RobotHandler rh;
    // Start is called before the first frame update
    void Start()
    {
        rh = transform.GetComponent<RobotHandler>();
    }
    

    void diff_cycle()
    {
        if (h < thresholdqr)
        {
            QR = true;
            rh.SpawnPheromone(0, h);
            //broadcast(0, h);
        }
        else
        {
            QR = false;
        }
    }

    void propagate_cycle()
    {
        if (received_pheromones.Count > 0)
        {
            // Initialize a list of size  received_pheromones.length
            float sum = 0;
            float result_pheromone = 0;
            float result_hopcount = 0;
            int i;    
            foreach (GameObject phero in received_pheromones)
            {
                PherormoneData pd = phero.GetComponent<PherormoneData>();
                if (pd.spawnerID != rh.robotID)
                {
                    if (pd.hopcount != 0)
                    {
                        sum += 1 / pd.hopcount;
                    }
                }
            }
            foreach (GameObject phero in received_pheromones)
            {
                PherormoneData pd = phero.GetComponent<PherormoneData>();
                if (pd.hopcount < threshold_hopcount && pd.spawnerID != rh.robotID)
                {
                    float weight = 0;
                    if (sum != 0)
                    {
                        weight = pd.hopcount / sum;
                        weight = 1 / weight;
                    }
                    //weight = 1 / weight;

                    result_pheromone = weight * pd.h;

                    result_hopcount = Mathf.Ceil(weight * pd.hopcount) ;    
                }
            }
            test_pheromone = result_pheromone;
            h += result_pheromone;
            //Broadcast the pheromone(Done externally)
            rh.SpawnPheromone(result_hopcount + 1, result_pheromone * hopcount_decay);
            //broadcast(results_hopcount + 1, result_pheromone * hopcount_decay);
        }
    }


    void move_cycle()
    {
        list_count = received_pheromones.Count;
        int counter = 0;
        foreach(GameObject phero in received_pheromones)
        {
            PherormoneData pd = phero.GetComponent<PherormoneData>();
            if (pd.spawnerID == rh.robotID && pd.pheromoneType == PheromoneTypes.Bee)
            {
                counter++;
            }
        }

        if (list_count > counter)
        {
            calculate_move();
            test = false;
        }
        else
        {
            test = true;
        }
        //Move the agent (Done externally)
        rh.RotateClockwise(current_theta_dest);
        if (Mathf.Floor(h) > 0 && Mathf.Floor(current_magnitude) > 0)
        {
            rh.MoveForward(Mathf.Floor(current_magnitude));
            current_magnitude -= 1f;
        }
        current_theta_dest = 0f;
    }

    void calculate_move()
    {
        float sum_x = 0f;
        float sum_y = 0f;
        float theta, diffx, diffy;
        float component_x, component_y;
        if (Mathf.Floor(h) > 0)
        {
            foreach (GameObject phero in received_pheromones)
            {
                PherormoneData pd = phero.GetComponent<PherormoneData>();
                if(pd.pheromoneType == PheromoneTypes.Bee && pd.spawnerID != rh.robotID)
                {
                    diffx = (phero.transform.position.x - transform.position.x);
                    diffy = (phero.transform.position.y - transform.position.x);
                    theta = rad2degrees(Mathf.Atan2(diffy, diffx));
                    component_x = pd.h * Mathf.Cos(theta);
                    component_y = pd.h * Mathf.Sin(theta);
                    sum_x += component_x;
                    sum_y += component_y;

                }
            }
        }
        float magnitude = Mathf.Sqrt(Mathf.Pow(sum_x,2) + Mathf.Pow(sum_y,2));
        float theta_dest = rad2degrees(Mathf.Atan2(sum_y, sum_x));
        theta_dest = theta_dest - 180f;
        received_pheromones.Clear();
        current_magnitude = magnitude;
        current_theta_dest = theta_dest;
    }

    void decay_cycle()
    {
        h = h * time_decay;
    }

    // Call this in each time step
    void operate()
    {

        if(calc_round == true) {
            diff_cycle();
            propagate_cycle();
            calc_round = false;
        }
        else {
            move_cycle();
            if (current_time_step == time_decay_steps)
            {
                decay_cycle();
                current_time_step = 0;

            }
            current_time_step++;
            calc_round = true;
        }
        //current_theta_dest = 0;
        //current_magnitude = 0;
    }
    

    public void Behavior()
    {
        List<GameObject> pheros = rh.ScanPheromone("PheromoneArea");
        /**foreach (GameObject phero in pheros)
        {
            PherormoneData pd = phero.GetComponent<PherormoneData>();
            if (pd.pheromoneType == PheromoneTypes.Bee && pd.spawnerID == rh.robotID)
            {
                Predicate<GameObject> isMine = IsMine;
                pheros.RemoveAll(isMine);
            }
        }**/
        received_pheromones = pheros;
        operate();
        /***foreach (GameObject phero in pheros)
        {
            PherormoneData pd = phero.GetComponent<PherormoneData>();
            if (pd.pheromoneType == PheromoneTypes.Bee)
            {
                
            }

            //rh.SpawnPheromone(2.5f);

            //transform.position.x; My X axis value
            //phero.transform.position.x; The pherormone's X axis value
            //Use similar for Z;
            //Z axis points forward, X points to the right, Y points up
            //transform.LookAt(-phero.transform.position); Look away from a pherormone. Use without the minus to look towards the pherormone
            //Mathf.Abs(transform.position.x);


        }***/
    }

    bool IsMine(GameObject pd)
    {
        return (pd.GetComponent<PherormoneData>().spawnerID == rh.robotID);
    }

    public float rad2degrees(float rad)
    {
        float angle = rad * (180 / Mathf.PI);
        return angle;
    }


}
