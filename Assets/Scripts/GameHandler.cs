using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameObject selectedRobot;
    public static GameObject selectedPherormone;

    public GameObject currentLevel;
    public GameObject[] LevelsListPrefabs;

    [Header("UI objects")]
    public GameObject LvlSelectDropdown;
    public Text RobotInfoText;
    public Text PherormoneInfoText;

    [Header("Execution Behavior")]
    public float executionClock = 0.5f;

    [Header("Handled Automatically")]
    public List<GameObject> robots = new List<GameObject>();

    int IDcounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        //UI Init
        Dropdown lvlList = LvlSelectDropdown.GetComponent<Dropdown>();
        lvlList.options.Clear();
        foreach (GameObject tmp in LevelsListPrefabs)
        {
            Dropdown.OptionData tmpD = new Dropdown.OptionData(tmp.name);
            lvlList.options.Add(tmpD);
        }

        InitGameData();

    }

    private void InitGameData()
    {
        selectedRobot = null;
        IDcounter = 0;
        GameObject[] initRobots = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject rob in initRobots)
        {
            robots.Add(rob);
            rob.GetComponent<RobotHandler>().robotID = IDcounter;
            IDcounter++;
        }


    }

    // Update is called once per frame
    void Update()
    {
        setRobotInfo();
        setPherormoneInfo();
    }

    //UI functions
    private void setRobotInfo()
    {
        if (selectedRobot == null)
        {
            RobotInfoText.text = "No robot selected";
        }
        else
        {
            RobotHandler rh = selectedRobot.GetComponent<RobotHandler>();
            RobotInfoText.text = "Robot " + selectedRobot.name + ". ID=" + rh.robotID + ". State=" + rh.state + ".";
        }
    }

    private void setPherormoneInfo()
    {
        if (selectedPherormone == null)
        {
            PherormoneInfoText.text = "No Pherormone selected";
        }
        else
        {
            PherormoneData pd = selectedPherormone.GetComponent<PherormoneData>();
            PherormoneInfoText.text = "Pheroromone " + selectedPherormone.name + ". Pherormone type=" + pd.pheromoneType + ". Spawned By ID=" + pd.spawnerID + ". Intensity=" + pd.intensity + ". Hopcount=" + pd.hopcount + ". H=" + pd.h + ".";
}
    }

    //Button Functions
    public void SwitchLevel()
    {
        foreach (GameObject tmp in robots)
        {
            Destroy(tmp);
        }
        GameObject[] pherormones = GameObject.FindGameObjectsWithTag("Pheromone");
        foreach (GameObject tmp in pherormones)
        {
            Destroy(tmp);
        }
        Destroy(currentLevel);

        currentLevel= Instantiate(LevelsListPrefabs[LvlSelectDropdown.GetComponent<Dropdown>().value], Vector3.zero, Quaternion.Euler(Vector3.zero));

        InitGameData();
    }

    public void Pause()
    {
        if (Time.timeScale != 0.0f)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public void FastForward()
    {
        if (Time.timeScale != 2.0f)
            Time.timeScale = 2.0f;
        else
            Time.timeScale = 1.0f;
    }


    //Routed execution
    public void RoundStart()
    {
        int i = 0;
        while (i < robots.Count)
        {
            if (robots[i] == null)
                robots.RemoveAt(i);
            else
                i++;

        }
        foreach (GameObject robot in robots)
        {
            robot.GetComponent<RobotHandler>().ExecuteBehavior();
        }
        Invoke("DoActions", executionClock);
    }

    public void DoActions()
    {
        int activeRobots = 0;
        RobotHandler rh;
        foreach (GameObject robot in robots)
        {
            if(robot == null)
            {
                break;
            }
            rh = robot.GetComponent<RobotHandler>();
            if (rh.robotMovesList.Count > 0)
            {
                if (rh.robotMovesList.Count > 1)
                    activeRobots++;
                rh.ExecuteMove();
            }
        }
        if (activeRobots > 0)
            Invoke("DoActions", executionClock);
        else
            Invoke("UpdatePhero", executionClock);
    }

    public void UpdatePhero()
    {
         GameObject[] pherormones = GameObject.FindGameObjectsWithTag("Pheromone");
        foreach (GameObject phero in pherormones)
        {
                phero.GetComponent<PherormoneData>().EndOfRoundUpdate();
        }
        Invoke("RoundStart", executionClock);
    }
}
