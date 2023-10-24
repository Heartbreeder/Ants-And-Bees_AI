using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaV2Behavior : MonoBehaviour
{
    RobotHandler rh;
    // Start is called before the first frame update
    void Start()
    {
        rh = transform.GetComponent<RobotHandler>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Behavior()
    {

        if (rh.ScanPheromone("forward").Count > 0)
        {
            float i = Random.Range(0, 1);
            if (i < 0.5f)
            {
                rh.RotateClockwise(Random.Range(5, 180));
            }
            else
            {
                rh.RotateClockwise(Random.Range(5, 180));
            }
        }
        //rh.RotateClockwise(Random.Range(5, 180));
        rh.MoveForward(1);
        rh.SpawnPheromone(2f);

    }




}
