using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerMaze : MonoBehaviour
{

    public struct Cell
    {
        public int Value;
        public bool UpGate;
        public bool DownGate;
        public bool LeftGate;
        public bool RightGate;

        public void setOriginal()
        {
            this.Value = 0;
            this.UpGate = false;
            this.DownGate = false;
            this.LeftGate = false;
            this.RightGate = false;
        }
    }

    Cell[,] Maze;
    short level;
    Vector2 gatePos;
    Vector2[] way;
    int[] posZero = new int[2];
    int numStep = 0;
    public int numStepMax;
    bool CheckFirstSupStep = true;
    bool CheckLastStep = false;
    List<int[]> mainWay = new List<int[]>();

    List<int[]> hintWay = new List<int[]>();
    public GameObject Bug;

    public int[] posBug = new int[2];
    int[] posHinted = { 77, 77 };
    int numStepToGate;

    public GameObject RedGate;

    public GameObject hintLine;
    public GameObject Canvas;
    public GameObject DataManager;
    float time;

    public int[] moveVector = new int[2];

    int valueHintCell;
    bool isFirstTouch = true;
    bool isMoved = false;
    bool isWin = false;
    float timeMove = 0;
    bool isAutoMoving = false;
    Vector2 posTouchZero = new Vector2(0f, 0f);
    Vector2 posTouchEnd = new Vector2(0f, 0f);


    DataProcessing DtP;


    // Start is called before the first frame update
    void Start()
    {
        isWin = false;
        isAutoMoving=false;
        timeMove = 0;
        DataManager = GameObject.Find("DataManager");
        DtP = DataManager.GetComponent<DataProcessing>();
        DtP.LoadGame();
        numStepMax = 20 + (int)(DtP.currentLV * 110 / 999);
        time = 0f;
        Canvas.transform.Find("Header").Find("ButtonHint").Find("Text").GetComponent<Text>().text = DtP.numHint.ToString();
        if (Screen.height / Screen.width < 2)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(0.84f, 0.84f, 1f);
        }
        posBug[0] = 0;
        posBug[1] = 0;
        Bug.transform.position = this.transform.GetChild(posBug[0]).GetChild(posBug[1]).position + new Vector3(0f, 0f, -2f);


        Maze = new Cell[13, 10];
        CreateMaze();
        DigCells(posZero);
        RedGate.transform.position = this.transform.GetChild(mainWay[mainWay.Count - 1][0]).GetChild(mainWay[mainWay.Count - 1][1]).position + new Vector3(0f, 0f, -1f);
        BreakWalls();

    }

    public void CreateNewMaze()
    {
        timeMove = 0;
        numStep = 0;
        isWin = false;
        isAutoMoving=false;
        Maze = new Cell[13, 10];
        posZero = new int[2];
        posBug[0] = 0;
        posBug[1] = 0;
        posHinted[0] = 77;
        posHinted[1] = 77;
        hintLine.GetComponent<LineRenderer>().positionCount = 0;
        Bug.transform.position = this.transform.GetChild(posBug[0]).GetChild(posBug[1]).position + new Vector3(0f, 0f, -2f);
        DigCells(posZero);
        RedGate.transform.position = this.transform.GetChild(mainWay[mainWay.Count - 1][0]).GetChild(mainWay[mainWay.Count - 1][1]).position + new Vector3(0f, 0f, -1f);
        BreakWalls();
    }

    void CreateMaze()
    {
        for (int r = 0; r < Maze.GetLength(0); r++)
        {
            for (int c = 0; c < Maze.GetLength(1); c++)
            {
                Maze[r, c].setOriginal();
            }
        }
    }

    void DigCells(int[] posStart)
    {
        mainWay.Clear();
        mainDigCell(posStart, 7);
        CheckLastStep = true;
        for (int nCell = 0; nCell < mainWay.Count - 1; nCell++)
        {
            CheckFirstSupStep = true;
            if (UnityEngine.Random.Range(0, (int)numStepMax) < (int)numStepMax / 2)
            {
                supDigCell(mainWay[nCell], 7);
            }
        }
        bool checkclearNullCell = false;
        while (checkclearNullCell == false)
        {
            int[] cell = new int[2];
            checkclearNullCell = true;
            for (int r = 0; r < Maze.GetLength(0); r++)
            {
                for (int c = 0; c < Maze.GetLength(1); c++)
                {
                    if (Maze[r, c].Value == 0)
                    {


                        cell[0] = r;
                        cell[1] = c;
                        checkclearNullCell = false;
                        break;
                    }
                }
                if (!checkclearNullCell)
                {
                    break;
                }
            }
            CheckLastStep = false;
            supDigCell(cell, 7);
        }
    }

    void showCellPos(int[] Cell)
    {
        string s = Cell[0].ToString() + "||" + Cell[1].ToString();
        Debug.Log(s);
    }

    void mainDigCell(int[] currentCell, int directIn)
    {
        if (((currentCell[0] >= 0) & (currentCell[0] < Maze.GetLength(0))) & ((currentCell[1] >= 0) & (currentCell[1] < Maze.GetLength(1))))
        {
            if (Maze[currentCell[0], currentCell[1]].Value == 0)
            {
                Maze[currentCell[0], currentCell[1]].Value = 1;
                switch (directIn)
                {
                    case 0:
                        Maze[currentCell[0], currentCell[1]].DownGate = true;
                        break;
                    case 1:
                        Maze[currentCell[0], currentCell[1]].LeftGate = true;
                        break;
                    case 2:
                        Maze[currentCell[0], currentCell[1]].UpGate = true;
                        break;
                    case 3:
                        Maze[currentCell[0], currentCell[1]].RightGate = true;
                        break;
                    default:
                        break;
                }
                int[] copyCell = new int[2];
                copyCell[0] = currentCell[0] + 0;
                copyCell[1] = currentCell[1] + 0;
                mainWay.Add(copyCell);
                int[] directs = { 0, 1, 2, 3 };
                for (int numDirectCheck = 0; numDirectCheck < directs.Length; numDirectCheck++)
                {
                    bool checkDirect = false;
                    int[] nextCellCheck = new int[2];
                    switch (directs[numDirectCheck])
                    {
                        case 0:
                            nextCellCheck[0] = currentCell[0] - 1;
                            nextCellCheck[1] = currentCell[1];

                            break;
                        case 1:
                            nextCellCheck[0] = currentCell[0];
                            nextCellCheck[1] = currentCell[1] + 1;
                            break;
                        case 2:
                            nextCellCheck[0] = currentCell[0] + 1;
                            nextCellCheck[1] = currentCell[1];
                            break;
                        default:
                            nextCellCheck[0] = currentCell[0];
                            nextCellCheck[1] = currentCell[1] - 1;
                            break;
                    }
                    if (((nextCellCheck[0] >= 0) & (nextCellCheck[0] < Maze.GetLength(0))) & ((nextCellCheck[1] >= 0) & (nextCellCheck[1] < Maze.GetLength(1))))
                    {
                        if (Maze[nextCellCheck[0], nextCellCheck[1]].Value == 0)
                        {
                            checkDirect = true;
                        }
                    }
                    if (!checkDirect)
                    {
                        List<int> directsList = new List<int>(directs);
                        directsList.RemoveAt(numDirectCheck);
                        directs = directsList.ToArray();
                        numDirectCheck--;
                    }
                }
                int[] nextCell = new int[2];
                int numDirect = 0;
                if (directs.Length > 0)
                {
                    numStep++;
                    numDirect = UnityEngine.Random.Range(0, directs.Length);
                    switch (directs[numDirect])
                    {
                        case 0:
                            nextCell[0] = currentCell[0] - 1;
                            nextCell[1] = currentCell[1];
                            if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                            {
                                Maze[currentCell[0], currentCell[1]].UpGate = true;
                            }
                            break;
                        case 1:
                            nextCell[0] = currentCell[0];
                            nextCell[1] = currentCell[1] + 1;
                            if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                            {
                                Maze[currentCell[0], currentCell[1]].RightGate = true;
                            }
                            break;
                        case 2:
                            nextCell[0] = currentCell[0] + 1;
                            nextCell[1] = currentCell[1];
                            if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                            {
                                Maze[currentCell[0], currentCell[1]].DownGate = true;
                            }
                            break;
                        default:
                            nextCell[0] = currentCell[0];
                            nextCell[1] = currentCell[1] - 1;
                            if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                            {
                                Maze[currentCell[0], currentCell[1]].LeftGate = true;
                            }
                            break;
                    }
                }
                else
                {
                    Maze[currentCell[0], currentCell[1]].Value = 17;
                }
                if (numStep <= numStepMax)
                {
                    if (directs.Length > 0)
                    {
                        if (((currentCell[0] >= 0) & (currentCell[0] < Maze.GetLength(0))) & ((currentCell[1] >= 0) & (currentCell[1] < Maze.GetLength(1))))
                        {
                            mainDigCell(nextCell, directs[numDirect]);
                        }
                    }
                }
                else
                {
                    Maze[currentCell[0], currentCell[1]].Value = 17;
                }

            }
        }
    }

    void supDigCell(int[] currentCell, int directIn)
    {
        if (((currentCell[0] >= 0) & (currentCell[0] < Maze.GetLength(0))) & ((currentCell[1] >= 0) & (currentCell[1] < Maze.GetLength(1))))
        {
            if (Maze[currentCell[0], currentCell[1]].Value == 0 | CheckFirstSupStep | !CheckLastStep)
            {
                if (Maze[currentCell[0], currentCell[1]].Value != 0)
                {
                    if (!CheckLastStep)
                    {
                        CheckLastStep = true;
                    }

                }
                switch (directIn)
                {
                    case 0:
                        Maze[currentCell[0], currentCell[1]].DownGate = true;
                        break;
                    case 1:
                        Maze[currentCell[0], currentCell[1]].LeftGate = true;
                        break;
                    case 2:
                        Maze[currentCell[0], currentCell[1]].UpGate = true;
                        break;
                    case 3:
                        Maze[currentCell[0], currentCell[1]].RightGate = true;
                        break;
                    default:
                        break;
                }
                switch (Maze[currentCell[0], currentCell[1]].Value)
                {
                    case 0:
                        if (!CheckLastStep)
                        {
                            Maze[currentCell[0], currentCell[1]].Value = 4;
                        }
                        else
                        {
                            Maze[currentCell[0], currentCell[1]].Value = 2;

                        }
                        break;
                    case 1:
                        Maze[currentCell[0], currentCell[1]].Value = 3;
                        break;
                    default:
                        Debug.Log(Maze[currentCell[0], currentCell[1]].Value);
                        break;
                }
                if (CheckFirstSupStep)
                {
                    CheckFirstSupStep = false;
                }
                List<int> directsList;
                int[] directs = { 0, 1, 2, 3 };
                for (int numDirectCheck = 0; numDirectCheck < directs.Length; numDirectCheck++)
                {
                    bool checkDirect = false;
                    int[] nextCellCheck = new int[2];
                    switch (directs[numDirectCheck])
                    {
                        case 0:
                            nextCellCheck[0] = currentCell[0] - 1;
                            nextCellCheck[1] = currentCell[1];

                            break;
                        case 1:
                            nextCellCheck[0] = currentCell[0];
                            nextCellCheck[1] = currentCell[1] + 1;
                            break;
                        case 2:
                            nextCellCheck[0] = currentCell[0] + 1;
                            nextCellCheck[1] = currentCell[1];
                            break;
                        default:
                            nextCellCheck[0] = currentCell[0];
                            nextCellCheck[1] = currentCell[1] - 1;
                            break;
                    }
                    if (((nextCellCheck[0] >= 0) & (nextCellCheck[0] < Maze.GetLength(0))) & ((nextCellCheck[1] >= 0) & (nextCellCheck[1] < Maze.GetLength(1))))
                    {
                        if (Maze[nextCellCheck[0], nextCellCheck[1]].Value == 0 | !CheckLastStep | CheckFirstSupStep)
                        {
                            if (Maze[nextCellCheck[0], nextCellCheck[1]].Value != 17)
                            {
                                checkDirect = true;
                                if (!CheckLastStep)
                                {
                                    if (Maze[nextCellCheck[0], nextCellCheck[1]].Value == 4 & Maze[currentCell[0], currentCell[1]].Value == 4)
                                    {
                                        checkDirect = false;
                                    }
                                }
                            }

                        }
                    }


                    if (!checkDirect)
                    {
                        directsList = new List<int>(directs);
                        directsList.RemoveAt(numDirectCheck);
                        directs = directsList.ToArray();
                        numDirectCheck--;
                    }
                }

                int[] nextCell = new int[2];
                int numDirect = 0;
                if (directs.Length > 0)
                {
                    int timeDig = UnityEngine.Random.Range(1, directs.Length + 1);
                    for (int t = 0; t < timeDig; t++)
                    {
                        numDirect = UnityEngine.Random.Range(0, directs.Length);
                        switch (directs[numDirect])
                        {
                            case 0:
                                nextCell[0] = currentCell[0] - 1;
                                nextCell[1] = currentCell[1];
                                if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                                {
                                    Maze[currentCell[0], currentCell[1]].UpGate = true;
                                }
                                break;
                            case 1:
                                nextCell[0] = currentCell[0];
                                nextCell[1] = currentCell[1] + 1;
                                if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                                {
                                    Maze[currentCell[0], currentCell[1]].RightGate = true;
                                }
                                break;
                            case 2:
                                nextCell[0] = currentCell[0] + 1;
                                nextCell[1] = currentCell[1];
                                if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                                {
                                    Maze[currentCell[0], currentCell[1]].DownGate = true;
                                }
                                break;
                            default:
                                nextCell[0] = currentCell[0];
                                nextCell[1] = currentCell[1] - 1;
                                if (((nextCell[0] >= 0) & (nextCell[0] < Maze.GetLength(0))) & ((nextCell[1] >= 0) & (nextCell[1] < Maze.GetLength(1))))
                                {
                                    Maze[currentCell[0], currentCell[1]].LeftGate = true;
                                }
                                break;
                        }
                        if (directs.Length > 0)
                        {
                            if (((currentCell[0] >= 0) & (currentCell[0] < Maze.GetLength(0))) & ((currentCell[1] >= 0) & (currentCell[1] < Maze.GetLength(1))))
                            {
                                supDigCell(nextCell, directs[numDirect]);
                            }
                        }
                        directsList = new List<int>(directs);
                        directsList.RemoveAt(numDirect);
                        directs = directsList.ToArray();
                    }
                }



            }
        }
    }
    void showMazeValue()
    {
        Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
        string s;
        for (int r = 0; r < Maze.GetLength(0); r++)
        {
            s = "";
            for (int c = 0; c < Maze.GetLength(1); c++)
            {
                s = s + Maze[r, c].Value.ToString() + "||";
            }
            Debug.Log(s);
        }
        Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");

    }


    void showMaze()
    {
        Color color;
        for (int r = 0; r < Maze.GetLength(0); r++)
        {
            for (int c = 0; c < Maze.GetLength(1); c++)
            {
                switch (Maze[r, c].Value)
                {
                    case 0:
                        color = Color.green;
                        break;
                    case 17:
                        color = Color.red;
                        break;
                    case 2:
                        color = Color.blue;
                        break;
                    case 1:
                        color = Color.yellow;
                        break;
                    case 4:
                        color = Color.black;
                        break;

                    default:
                        color = Color.white;
                        break;
                }
                this.transform.GetChild(r).GetChild(c).GetChild(0).GetComponent<SpriteRenderer>().color = color;
                if (Maze[r, c].Value / 70 == 1)
                {
                    this.transform.GetChild(r).GetChild(c).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(Mathf.Pow(0.7f, Maze[r, c].Value % 70), 0f, 0f);
                }
            }
        }
    }

    void BreakWalls()
    {
        for (int r = 0; r < Maze.GetLength(0); r++)
        {
            for (int c = 0; c < Maze.GetLength(1); c++)
            {
                this.transform.GetChild(r).GetChild(c).GetChild(0).gameObject.SetActive(!Maze[r, c].UpGate);
                this.transform.GetChild(r).GetChild(c).GetChild(1).gameObject.SetActive(!Maze[r, c].RightGate);
                this.transform.GetChild(r).GetChild(c).GetChild(2).gameObject.SetActive(!Maze[r, c].DownGate);
                this.transform.GetChild(r).GetChild(c).GetChild(3).gameObject.SetActive(!Maze[r, c].LeftGate);
            }
        }

    }

    public void Move()
    {
        if (Input.touchCount > 0)
        {
            if (isFirstTouch & Input.GetTouch(0).position.y < Screen.height * 2 / 3)
            {
                posTouchZero = Input.GetTouch(0).position;
                //Debug.Log(posTouchZero.x.ToString()+"||"+posTouchZero.y.ToString());

                isFirstTouch = false;
                isMoved = false;
            }
            else
            {
                posTouchEnd = Input.GetTouch(0).position;
                //Debug.Log(posTouchEnd.x.ToString()+"||"+posTouchEnd.y.ToString());
            }
        }
        else
        {
            if (!isMoved)
            {
                if (Mathf.Abs(posTouchEnd.x - posTouchZero.x) >= Mathf.Abs(posTouchEnd.y - posTouchZero.y))
                {
                    if (posTouchEnd.x - posTouchZero.x > 0)
                    {
                        if (((posBug[1] + 1 >= 0) & (posBug[1] + 1 < Maze.GetLength(1))))
                        {
                            if (Maze[posBug[0], posBug[1]].RightGate & Maze[posBug[0], posBug[1] + 1].LeftGate)
                            {
                                posBug[1]++;
                                Bug.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                            }
                        }
                    }
                    else
                    {
                        if (((posBug[1] - 1 >= 0) & (posBug[1] - 1 < Maze.GetLength(1))))
                        {
                            if (Maze[posBug[0], posBug[1]].LeftGate & Maze[posBug[0], posBug[1] - 1].RightGate)
                            {
                                posBug[1]--;
                                Bug.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                            }
                        }
                    }
                }
                else
                {
                    if (posTouchEnd.y - posTouchZero.y > 0)
                    {
                        if (((posBug[0] - 1 >= 0) & (posBug[0] - 1 < Maze.GetLength(0))))
                        {
                            if (Maze[posBug[0], posBug[1]].UpGate & Maze[posBug[0] - 1, posBug[1]].DownGate)
                            {
                                posBug[0]--;
                                Bug.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            }
                        }
                    }
                    else
                    {
                        if (((posBug[0] + 1 >= 0) & (posBug[0] + 1 < Maze.GetLength(0))))
                        {
                            if (Maze[posBug[0], posBug[1]].DownGate & Maze[posBug[0] + 1, posBug[1]].UpGate)
                            {
                                posBug[0]++;
                                Bug.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                            }
                        }
                    }
                }

                isFirstTouch = true;
                isMoved = true;
                Bug.transform.position = this.transform.GetChild(posBug[0]).GetChild(posBug[1]).position + new Vector3(0f, 0f, -2f);
            }
        }


    }

    void CheckWin()
    {

        if (posBug[0] == mainWay[mainWay.Count - 1][0] & posBug[1] == mainWay[mainWay.Count - 1][1])
        {
            isWin = true;
            Canvas.transform.Find("NextLvMenu").gameObject.SetActive(true);
        }

    }

    void findHintWay(int[] currentCell)
    {
        if (hintWay.Count > 0)
        {
            if (hintWay[hintWay.Count - 1][0] == 777)
            {
                int[] copyCell = new int[2];
                copyCell[0] = currentCell[0] + 0;
                copyCell[1] = currentCell[1] + 0;
                hintWay.Add(copyCell);

            }
            else
            {
                if (Maze[hintWay[hintWay.Count - 1][0], hintWay[hintWay.Count - 1][1]].Value != 17)
                {
                    int[] copyCell = new int[2];
                    copyCell[0] = currentCell[0] + 0;
                    copyCell[1] = currentCell[1] + 0;
                    hintWay.Add(copyCell);

                }
            }
        }
        else
        {
            int[] copyCell = new int[2];
            copyCell[0] = currentCell[0] + 0;
            copyCell[1] = currentCell[1] + 0;
            hintWay.Add(copyCell);
        }
        if (Maze[hintWay[hintWay.Count - 1][0], hintWay[hintWay.Count - 1][1]].Value != 17)
        {
            int[] directs = { 0, 1, 2, 3 };
            for (int numDirectCheck = 0; numDirectCheck < directs.Length; numDirectCheck++)
            {
                bool checkDirect = false;
                int[] nextCellCheck = new int[2];
                switch (directs[numDirectCheck])
                {
                    case 0:
                        nextCellCheck[0] = currentCell[0] - 1;
                        nextCellCheck[1] = currentCell[1];
                        break;
                    case 1:
                        nextCellCheck[0] = currentCell[0];
                        nextCellCheck[1] = currentCell[1] + 1;
                        break;
                    case 2:
                        nextCellCheck[0] = currentCell[0] + 1;
                        nextCellCheck[1] = currentCell[1];
                        break;
                    default:
                        nextCellCheck[0] = currentCell[0];
                        nextCellCheck[1] = currentCell[1] - 1;
                        break;
                }
                if (((nextCellCheck[0] >= 0) & (nextCellCheck[0] < Maze.GetLength(0))) & ((nextCellCheck[1] >= 0) & (nextCellCheck[1] < Maze.GetLength(1))))
                {
                    switch (directs[numDirectCheck])
                    {
                        case 0:
                            if (Maze[currentCell[0], currentCell[1]].UpGate & Maze[nextCellCheck[0], nextCellCheck[1]].DownGate)
                            {
                                checkDirect = true;
                            }
                            break;
                        case 1:
                            if (Maze[currentCell[0], currentCell[1]].RightGate & Maze[nextCellCheck[0], nextCellCheck[1]].LeftGate)
                            {
                                checkDirect = true;
                            }
                            break;
                        case 2:
                            if (Maze[currentCell[0], currentCell[1]].DownGate & Maze[nextCellCheck[0], nextCellCheck[1]].UpGate)
                            {
                                checkDirect = true;
                            }
                            break;
                        default:
                            if (Maze[currentCell[0], currentCell[1]].LeftGate & Maze[nextCellCheck[0], nextCellCheck[1]].RightGate)
                            {
                                checkDirect = true;
                            }
                            break;
                    }
                    foreach (int[] cell in hintWay)
                    {
                        if (nextCellCheck[0] == cell[0] & nextCellCheck[1] == cell[1])
                        {
                            checkDirect = false;
                            break;
                        }
                    }
                }
                if (!checkDirect)
                {
                    List<int> directsList = new List<int>(directs);
                    directsList.RemoveAt(numDirectCheck);
                    directs = directsList.ToArray();
                    numDirectCheck--;
                }
            }
            int[] nextCell = new int[2];
            if (directs.Length > 0)
            {
                if (directs.Length > 1)
                {
                    hintWay.Add(new int[] { 777, 777 });
                }
                for (int numDirect = 0; numDirect < directs.Length; numDirect++)
                {
                    switch (directs[numDirect])
                    {
                        case 0:
                            nextCell[0] = currentCell[0] - 1;
                            nextCell[1] = currentCell[1];
                            break;
                        case 1:
                            nextCell[0] = currentCell[0];
                            nextCell[1] = currentCell[1] + 1;
                            break;
                        case 2:
                            nextCell[0] = currentCell[0] + 1;
                            nextCell[1] = currentCell[1];
                            break;
                        default:
                            nextCell[0] = currentCell[0];
                            nextCell[1] = currentCell[1] - 1;
                            break;
                    }
                    findHintWay(nextCell);
                }
                if (directs.Length > 1)
                {
                    if (hintWay.Count > 0)
                    {
                        if (hintWay[hintWay.Count - 1][0] == 777)
                        {
                            hintWay.RemoveAt(hintWay.Count - 1);
                            while (hintWay[hintWay.Count - 1][0] != 777 & hintWay.Count > 0)
                            {
                                hintWay.RemoveAt(hintWay.Count - 1);
                            }
                        }

                    }


                }
            }
            else
            {
                if (Maze[hintWay[hintWay.Count - 1][0], hintWay[hintWay.Count - 1][1]].Value != 17)
                {
                    while (hintWay[hintWay.Count - 1][0] != 777 & hintWay.Count > 0)
                    {
                        hintWay.RemoveAt(hintWay.Count - 1);
                    }
                }

            }
        }
    }

    public void AutoWin()
    {
        if (!isAutoMoving)
        {
            hintWay.Clear();
            findHintWay(posBug);
            for (int numCell = 0; numCell < hintWay.Count; numCell++)
            {
                if (hintWay[numCell][0] == 777)
                {
                    hintWay.RemoveAt(numCell);
                    numCell--;
                }
            }
            isAutoMoving = true;
            numStepToGate = 0;
            Canvas.transform.Find("Header").Find("ButtonAutoMove").Find("Text").gameObject.GetComponent<Text>().text = "Moving...";
        }
    }

    void AutoMoveToGate()
    {
        if (isAutoMoving&!isWin)
        {
            timeMove += Time.deltaTime;
            if (timeMove > 0.7f)
            {
                numStepToGate += 1;
                bool isChangeRot=false;
                Vector3 rot=new Vector3(0f, 0f, 90f);
                if(posBug[0]<hintWay[numStepToGate][0])
                {
                    rot=new Vector3(0f, 0f, 180f);
                    isChangeRot=true;
                }
                if(posBug[0]>hintWay[numStepToGate][0]&!isChangeRot)
                {
                    rot=new Vector3(0f, 0f, 0f);
                    isChangeRot=true;
                }
                if(posBug[1]<hintWay[numStepToGate][1]&!isChangeRot)
                {
                    rot=new Vector3(0f, 0f, -90f);
                }
                if(posBug[1]>hintWay[numStepToGate][1]&!isChangeRot)
                {
                    rot=new Vector3(0f, 0f, 90f);
                }
                posBug[0] = hintWay[numStepToGate][0];
                posBug[1] = hintWay[numStepToGate][1];
                Bug.transform.position = this.transform.GetChild(posBug[0]).GetChild(posBug[1]).position + new Vector3(0f, 0f, -2f);
                Bug.transform.rotation = Quaternion.Euler(rot);
                timeMove = 0;
            }
        }
    }

    public void showHint()
    {
        if (DtP.numHint > 0 & !isAutoMoving)
        {
            if (posBug[0] != posHinted[0] & posBug[1] != posHinted[1])
            {
                DtP.numHint--;
                Canvas.transform.Find("Header").Find("ButtonHint").Find("Text").GetComponent<Text>().text = DtP.numHint.ToString();
                hintWay.Clear();
                findHintWay(posBug);
                for (int numCell = 0; numCell < hintWay.Count; numCell++)
                {
                    if (hintWay[numCell][0] == 777)
                    {
                        hintWay.RemoveAt(numCell);
                        numCell--;
                    }
                }
                hintLine.GetComponent<LineRenderer>().positionCount = hintWay.Count;
                for (int numCell = 0; numCell < hintWay.Count; numCell++)
                {

                    Vector3 cell = this.transform.GetChild(hintWay[numCell][0]).GetChild(hintWay[numCell][1]).position;
                    hintLine.GetComponent<LineRenderer>().SetPosition(numCell, new Vector3(cell.x, cell.y, 0f));

                }
                posHinted[0] = posBug[0] + 0;
                posHinted[1] = posBug[1] + 0;

            }

        }

    }

    public void GotoMenu()
    {
        DtP.SaveGame();
        SceneManager.LoadScene(0);
    }


    public void GotoNextLv()
    {
        if (DtP.lvReached == DtP.currentLV)
        {
            DtP.lvReached++;
        }
        DtP.currentLV++;
        DtP.SaveGame();
        SceneManager.LoadScene(1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }



    void Update()
    {
        if (!isWin & !isAutoMoving)
        {
            Move();
            if(time<180f)
            {
                time+=Time.deltaTime;
                Canvas.transform.Find("Header").Find("Timer").gameObject.GetComponent<Slider>().value=1f-time/180;
            } 
        }
        AutoMoveToGate();

        CheckWin();

    }
}
