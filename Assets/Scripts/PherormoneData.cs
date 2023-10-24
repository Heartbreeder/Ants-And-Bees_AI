using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PherormoneData : MonoBehaviour
{
    [Header("Set Manually")]
    public PheromoneTypes pheromoneType;
    public float decay;

    [Header("Set Automatically")]
    public int spawnerID;
    public float intensity;
    public float h;
    public float hopcount;


    public void setParameters(PheromoneTypes PheromoneType, int SpawnerID, float InitialIntensity)
    {
        pheromoneType = PheromoneType;
        spawnerID = SpawnerID;
        intensity = InitialIntensity;
    }

    public void setParameters(PheromoneTypes PheromoneType, int SpawnerID, float InitialIntensity, float Inithopcount)
    {
        pheromoneType = PheromoneType;
        spawnerID = SpawnerID;
        intensity = InitialIntensity;
        hopcount = Inithopcount;
    }

    public void EndOfRoundUpdate()
    {
        reduceIntensity(decay);
    }

    public void reduceIntensity(float value)
    {
        intensity -= value;
        if (intensity < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        GameHandler.selectedPherormone = gameObject;
    }

}

public enum PheromoneTypes
{
    Roomba,
    Ant,
    Bee
}
