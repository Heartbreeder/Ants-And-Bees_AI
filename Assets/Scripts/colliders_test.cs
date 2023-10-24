using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliders_test : MonoBehaviour
{

    //The list of colliders currently inside the trigger
    List<Collider> TriggerList = new List<Collider>();
 
    //called when something enters the trigger
    void OnTriggerEnter(Collider other)
    {
        //if the object is not already in the list
        if (!TriggerList.Contains(other) && other.gameObject.tag == "pherormone")
        {
            //add the object to the list
            TriggerList.Add(other);
        }
    }

    //called when something exits the trigger
    void OnTriggerExit(Collider other)
    {
        //if the object is in the list
        if (TriggerList.Contains(other) && other.gameObject.tag == "pherormone")
        {
            //remove it from the list
            TriggerList.Remove(other);
        }
    }

}
