using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHandler : MonoBehaviour
{
    public GameObject PherormonePrefab;
    public PheromoneTypes pheroType = PheromoneTypes.Roomba;


    [Header("Handled automatically")]
    public int robotID;
    public List<RobotMove> robotMovesList;

    private int loopCount = 0;
    [HideInInspector]
    public moves state;
    [HideInInspector]
    public Vector3 target;
    //[HideInInspector]
    public float speed=2;

    void Start()
    {
        state = moves.idle;
        robotMovesList = new List<RobotMove>();
    }


    void Update()
    {
        if (state == moves.move){
            float step = speed * Time.deltaTime;
            transform.Translate(Vector3.forward * step);
        }
    }

    //Fill the list of moves according to the robot's behavior
    public void ExecuteBehavior()
    {
        gameObject.SendMessage("Behavior");
    }

    //Execute one move
    public void ExecuteMove()
    {
        if (robotMovesList.Count == 0)
            return;
        gameObject.SendMessage("_" + robotMovesList[0].moveName, robotMovesList[0]);
        robotMovesList.RemoveAt(0);
        //gameObject.SendMessage("Behavior");
    }

    /*
     * Robot moves
     * */

    //Move Forward
    public void MoveForward(float distance)
    {
        
        RobotMove move = new RobotMove();
        move.moveName = "MoveForward";
        move.parameter = distance;
        robotMovesList.Add(move);
    }

    private void _MoveForward(RobotMove param)
    {
        float distance = param.parameter;
        Vector3 current = transform.position;
        target = transform.position + (transform.forward * distance);

        state = moves.move;
        Invoke("StopMoving", 0.5f);
    }

    private void StopMoving()
    {
        state = moves.idle;
    }

    //Rotate

    public void RotateClockwise(float degrees)
    {
        RobotMove move = new RobotMove();
        move.moveName = "RotateClockwise";
        move.parameter = degrees;
        robotMovesList.Add(move);
    }

    public void _RotateClockwise(RobotMove param)
    {
        float degrees = param.parameter;
        Vector3 angle = new Vector3(0, degrees, 0);
        transform.Rotate(angle);
    }

    public void RotateAntiClockwise(float degrees)
    {
        RobotMove move = new RobotMove();
        move.moveName = "RotateAntiClockwise";
        move.parameter = degrees;
        robotMovesList.Add(move);
    }

    public void _RotateAntiClockwise(RobotMove param)
    {
        float degrees = param.parameter;
        Vector3 angle = new Vector3(0, -degrees, 0);
        transform.Rotate(angle);
    }

    //Spawn Pherormone

    public void SpawnPheromone(float radius)
    {
        RobotMove move = new RobotMove();
        move.moveName = "SpawnPheromone";
        move.parameter = radius;
        move.parameter2 = -1;
        robotMovesList.Add(move);
    }

    public void SpawnPheromone(float radius, float h)
    {
        RobotMove move = new RobotMove();
        move.moveName = "SpawnPheromone";
        move.parameter = radius;
        move.parameter2 = h;
        robotMovesList.Add(move);
    }

    public void _SpawnPheromone(RobotMove param)
    {
        float radius = param.parameter;
        float h = param.parameter2;
        GameObject newPhero;
        if (PherormonePrefab != null)
        {
            Debug.Log("everything good");
            newPhero = Instantiate(PherormonePrefab, transform.position, transform.rotation);
            newPhero.GetComponent<PherormoneData>().h = h;
            //newPhero.GetComponent<SphereCollider>().radius = radius;
            if (pheroType != PheromoneTypes.Bee)
            {
                newPhero.transform.localScale = new Vector3(radius, radius, radius);
                newPhero.GetComponent<PherormoneData>().setParameters(pheroType, robotID, 1);  
            }else
            {
                Debug.Log("Should have spawned");
                newPhero.transform.localScale = new Vector3(3, 3, 3);
                newPhero.GetComponent<PherormoneData>().setParameters(pheroType, robotID, 1, radius);
                
            }
        }
        else
        {
            Debug.Log("Should not be here");
        }
    }

    /*
     * Sensor behaviors
     * */

    public List<GameObject> ScanPheromone (string sensorName)
    {
        for (int i=0; i< transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.name == sensorName)
            {
                return child.GetComponent<SensorBehavior>().GetPheromonesInSensor();
            }
        }
        return null;
    }

    public bool IsObstructed(string sensorName)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.name == sensorName)
            {
                return child.GetComponent<SensorBehavior>().obstructed();
            }
        }
        return false;
    }

    /*
     * Custom/misc behaviors
     * */

    public bool ScanWall (direction dir, float distance)
    {
        Vector3 towards;
        if (dir == direction.back){ towards = -Vector3.forward;}
        else if(dir == direction.right){ towards = Vector3.right;}
        else if (dir == direction.left) { towards = -Vector3.right; }
        else if (dir == direction.forwardLeft) { towards = new Vector3(-0.5f, 0, 0.5f); }
        else if (dir == direction.forwardRight) { towards = new Vector3(0.5f, 0, 0.5f); }
        else if (dir == direction.backLeft) { towards = new Vector3(-0.5f, 0, -0.5f); }
        else if (dir == direction.backRight) { towards = new Vector3(0.5f, 0, -0.5f); }
        else/*forward*/{ towards = -Vector3.forward; }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, towards, out hit, distance, LayerMask.GetMask("Wall")))
        {
            if (hit.collider.tag=="Wall")
                return true;
        }
        return false;
    }

    public bool ScanRobot(direction dir, float distance)
    {
        Vector3 towards;
        if (dir == direction.back) { towards = -Vector3.forward; }
        else if (dir == direction.right) { towards = Vector3.right; }
        else if (dir == direction.left) { towards = -Vector3.right; }
        else if (dir == direction.forwardLeft) { towards = new Vector3(-0.5f, 0, 0.5f); }
        else if (dir == direction.forwardRight) { towards = new Vector3(0.5f, 0, 0.5f); }
        else if (dir == direction.backLeft) { towards = new Vector3(-0.5f, 0, -0.5f); }
        else if (dir == direction.backRight) { towards = new Vector3(0.5f, 0, -0.5f); }
        else/*forward*/{ towards = -Vector3.forward; }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, towards, out hit, distance))
        {
            if (hit.collider.tag == "Robot")
                return true;
        }
        return false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        state = moves.idle;
        if (!collision.collider.isTrigger)
            transform.Translate(Vector3.back * 0.05f);
    }

    private void OnMouseDown()
    {
        GameHandler.selectedRobot = gameObject;
    }

}

public enum direction
{
    forward,
    back,
    left,
    right,
    forwardRight,
    forwardLeft,
    backRight,
    backLeft
}

public enum direction12
{
    SE,
    SW,
    NW,
    NE,
    ESE,
    ENE,
    WSW,
    WNW,
    SSW,
    SSE,
    NNW,
    NNE
}

public enum moves
{
    move,
    rotate,
    idle
}

public class RobotMove
{
    public string moveName;
    public float parameter;
    public float parameter2;

}
