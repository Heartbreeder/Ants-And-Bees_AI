using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public float speed = 1.0f;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public GameObject cam;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal")*speed, 0, Input.GetAxis("Vertical")*speed);
        transform.Translate(move);

        if (Input.GetButton("Jump"))
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(0, yaw, 0.0f);
            cam.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetKeyDown("q"))
        {
            move = new Vector3(0, 0.3f, 0);
            transform.Translate(move);
        }
        if (Input.GetKeyDown("e"))
        {
            move = new Vector3(0, -0.3f, 0);
            transform.Translate(move);
        }

        /*
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        */
    }
}
