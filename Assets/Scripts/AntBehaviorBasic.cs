using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehaviorBasic : MonoBehaviour
{

    RobotHandler rh;
    private bool move_clockwise;
    private int angle;      // rad/sec
    private int velocity;   // m/sec

    // Start is called before the first frame update
    void Start()
    {
        rh = transform.GetComponent<RobotHandler>();
        float i = Random.Range(0f, 2f);
        move_clockwise = (i < 1f) ? true : false;
        angle = 0;
        velocity = 1;
    }

    private void ReverseCicling()
    {
        move_clockwise = !move_clockwise;
        angle = 120;//170 + (int) Random.Range(0f,10f);
        velocity = 1;
    }
    private void RotateExterior()
    {
        angle = 50;
        velocity = 1;
      
        rh.MoveForward(velocity);
        if (move_clockwise){rh.RotateClockwise(angle);}
        else{rh.RotateAntiClockwise(angle);}
        rh.MoveForward(velocity);
        if (move_clockwise) { rh.RotateClockwise(angle); }
        else { rh.RotateAntiClockwise(angle); }
    }
    private void CircleAround()
    {
        angle = 50;
        velocity = 1;
    }

    public void Behavior()
    {


        /*
					 N
				NNW		NNE
				NW		NE
				WNW		ENE
			W				  E
				WSW		ESE
				SW		SE
				SSW		SSE
					 S
		*/
        //Scan pherormones in one trigger

        //Check clockwise


        List<string> interior_sensors = new List<string>();
        List<string> exterior_sensors = new List<string>();
        if (move_clockwise)
        {
            interior_sensors.Add("NNE");
            interior_sensors.Add("NE");
            interior_sensors.Add("ENE");

            exterior_sensors.Add("ESE");
            exterior_sensors.Add("SE");
            exterior_sensors.Add("SSE");
        }
        else
        {
            interior_sensors.Add("ESE");
            interior_sensors.Add("SE");
            interior_sensors.Add("SSE");

            exterior_sensors.Add("NNE");
            exterior_sensors.Add("NE");
            exterior_sensors.Add("ENE");
        }
        int pheroFound = 0;
        foreach (string interior in interior_sensors)
        {
            List<GameObject> pheroList = rh.ScanPheromone(interior);
            foreach (GameObject phero in pheroList)
            {
                PherormoneData pd = phero.GetComponent<PherormoneData>();
                if (pd.spawnerID != rh.robotID && pd.pheromoneType == PheromoneTypes.Ant)
                {
                    pheroFound += 1;
                    ReverseCicling();
                    break;
                }
            }
            if (pheroFound > 0) { break; }
        }
        if (pheroFound == 0)
        {
            foreach (string exterior in exterior_sensors)
            {
                List<GameObject> pheroList = rh.ScanPheromone(exterior);
                foreach (GameObject phero in pheroList)
                {
                    PherormoneData pd = phero.GetComponent<PherormoneData>();
                    if (pd.spawnerID != rh.robotID && pd.pheromoneType == PheromoneTypes.Ant)
                    {
                        pheroFound += 1;
                        RotateExterior();
                        break;
                    }
                }
                if (pheroFound > 0) { break; }
            }
        }
        if (pheroFound == 0)
        {//no foreing pheromone was found
            CircleAround();
        }
        //for eleven same code

        // rh.IsObstructed("NW");//returns true if there is an obstacle inside this sensor's range
        rh.MoveForward(velocity);//Move forward 1 meter
        if (move_clockwise)
        {
            rh.RotateClockwise(angle);//Rotate 10 degrees clockwise
        }
        else { rh.RotateAntiClockwise(angle); }
        rh.SpawnPheromone(1);

    }
}
