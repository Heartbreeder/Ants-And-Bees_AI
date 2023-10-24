using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myClass : MonoBehaviour
{
    public static myClass i;

    void Awake()
    {
        if (!i)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}