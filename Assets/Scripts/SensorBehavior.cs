using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorBehavior : MonoBehaviour
{
    public int obstaclesInRange;
    public List<GameObject> pherormonesInRange;
    
    private void Start()
    {
        pherormonesInRange = new List<GameObject>();
    }

    public bool obstructed()
    {
        if (obstaclesInRange > 0)
            return true;
        else
            return false;
    }

    public List<GameObject> GetPheromonesInSensor()
    {
        List<GameObject> ret = new List<GameObject>();
        int i = 0;
        while (i < pherormonesInRange.Count)
        {
            if (pherormonesInRange[i] == null)
                pherormonesInRange.RemoveAt(i);
            else
                i++;

        } 
        /*
        foreach(GameObject phero in pherormonesInRange)
        {
            if (phero == null)
                pherormonesInRange.Remove(phero);
            else
                ret.Add(phero);
        }*/
        return pherormonesInRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pheromone")
        {
            pherormonesInRange.Add(other.gameObject);
        }else if(other.tag=="Wall"|| other.tag == "Robot"|| other.tag == "Obstacle")
        {
            obstaclesInRange++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pheromone")
        {
            pherormonesInRange.Remove(other.gameObject);
        }
        else if (other.tag == "Wall" || other.tag == "Robot" || other.tag == "Obstacle")
        {
            obstaclesInRange--;
        }
    }




}
