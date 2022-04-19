using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContentControl : MonoBehaviour
{

    // Update is called once per frame
    public GameObject rowEvenContentPf;
    public GameObject rowOddContentPf;

    public GameObject rowLast;

    public GameObject DataManager;

    public GameObject numStars;

    public GameObject NotiLoading;

    public GameObject NotiLvLocked;

    public GameObject Canvas;

    public GameObject Pool;

    DataProcessing DtP;
    Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);


    void Start()
    {
        DtP = DataManager.GetComponent<DataProcessing>();
        if (DtP.isNewGame)
        {
            DtP.RandomStart();
            DtP.isNewGame = false;
        }
        else
        {
            DtP.LoadGame();
        }
        this.transform.localPosition = new Vector3(0f, DtP.yScrollMap, 0f);
        DtP.SaveGame();
        numStars.transform.Find("Text").GetComponent<Text>().text = DtP.numStars.ToString();
        SetUpRows();
    }

    public void Reset()
    {
        DtP.ResetGame();
        numStars.transform.Find("Text").GetComponent<Text>().text = DtP.numStars.ToString();
        this.transform.localPosition = new Vector3(0f, 0f, 0f);
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (this.transform.localPosition.y > 0)
        {
            this.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        if (this.transform.localPosition.y < -73700)
        {
            this.transform.localPosition = new Vector3(0f, -73700f, 0f);
        }

        LoadRow();
        ShowNotiLoading();
        //Debug.Log(Time.deltaTime);
        if (Input.touchCount > 0)
        {
            Debug.Log("v");
        }
    }

    public void showNotiLvLocked()
    {
        NotiLvLocked.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GotoLV(int level)
    {
        DtP.LoadGame();
        DtP.currentLV = level;
        DtP.yScrollMap = this.transform.localPosition.y;
        DtP.SaveGame();
        SceneManager.LoadScene(1);
    }

    public void GotoLvMax()
    {
        if (Screen.height / Screen.width < 2)
        {
            this.transform.localPosition = new Vector3(0f, (DtP.lvReached / 4) * -293f, 0f);
        }
        else
        {
            this.transform.localPosition = new Vector3(0f, (DtP.lvReached / 4) * -293f, 0f);
        }

    }

    public void RandomNewGame()
    {
        DtP.RandomStart();
        DtP.LoadGame();
        DtP.isNewGame = false;
        this.transform.localPosition = new Vector3(0f, DtP.yScrollMap, 0f);
        DtP.SaveGame();
        numStars.transform.Find("Text").GetComponent<Text>().text = DtP.numStars.ToString();
        SetUpRows();
    }
    void ShowNotiLoading()
    {
        if (Mathf.Abs(this.transform.GetChild(0).position.y) > 3000)
        {
            NotiLoading.SetActive(true);
        }
        else
        {
            NotiLoading.SetActive(false);
        }
    }



    void SetUpRows()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject rowProcessing = this.transform.GetChild(i).gameObject;
            for (int c = 0; c < 4; c++)
            {
                SetUpRow(rowProcessing);
            }
        }

    }

    void SetUpRow(GameObject Row)
    {
        int ColumnMax;
        if (Row.GetComponent<rowContentControl>().numRow == 249)
        {
            ColumnMax = 3;
        }
        else
        {
            ColumnMax = 4;
        }
        for (int numColumn = 0; numColumn < ColumnMax; numColumn++)
        {
            GameObject lvButtonProcessing = Row.transform.Find("LvButtons").GetChild(numColumn).gameObject;
            SetUpLvButtonLv(lvButtonProcessing, Row, numColumn);
            SetUpLvButtonLvText(lvButtonProcessing);
            SetUpLvButtonLvRate(lvButtonProcessing);
            SetUpLvButtonLvStatus(lvButtonProcessing);

        }
    }

    void SetUpLvButtonLv(GameObject LvButtonProcessing, GameObject Row, int column)
    {
        LvButtonProcessing.GetComponent<LvButton>().level = Row.GetComponent<rowContentControl>().numRow * 4 + column + 1;
    }

    void SetUpLvButtonLvText(GameObject LvButtonProcessing)
    {
        if (LvButtonProcessing.GetComponent<LvButton>().level != 1)
        {
            LvButtonProcessing.transform.GetChild(0).Find("LvText").gameObject.GetComponent<Text>().text = LvButtonProcessing.GetComponent<LvButton>().level.ToString();
            LvButtonProcessing.transform.GetChild(0).Find("LvText").gameObject.SetActive(true);
        }
        else
        {
            LvButtonProcessing.transform.GetChild(0).Find("LvText").gameObject.SetActive(false);
        }
    }

    void SetUpLvButtonLvRate(GameObject LvButtonProcessing)
    {
        //reset rate
        for (byte nStar = 0; nStar < 3; nStar++)
        {
            LvButtonProcessing.transform.GetChild(0).Find("Rate").GetChild(nStar).gameObject.SetActive(false);
        }

        //set new rate
        for (byte nStar = 0; nStar < DtP.lvStars[LvButtonProcessing.GetComponent<LvButton>().level - 1]; nStar++)
        {
            LvButtonProcessing.transform.GetChild(0).Find("Rate").GetChild(nStar).gameObject.SetActive(true);
        }
    }

    void SetUpLvButtonLvStatus(GameObject LvButtonProcessing)
    {
        // set up unlocked and locked button
        if (LvButtonProcessing.GetComponent<LvButton>().level <= (int)DtP.lvReached)
        {
            LvButtonProcessing.GetComponent<LvButton>().isUnlocked = true;
            LvButtonProcessing.transform.GetChild(0).gameObject.SetActive(true);
            LvButtonProcessing.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            LvButtonProcessing.GetComponent<LvButton>().isUnlocked = false;
            LvButtonProcessing.transform.GetChild(1).gameObject.SetActive(true);
            LvButtonProcessing.transform.GetChild(0).gameObject.SetActive(false);

        }

        // show image tutorial
        if (LvButtonProcessing.GetComponent<LvButton>().level == 1)
        {
            LvButtonProcessing.transform.GetChild(0).Find("imgTutorial").gameObject.SetActive(true);
        }
        else
        {
            LvButtonProcessing.transform.GetChild(0).Find("imgTutorial").gameObject.SetActive(false);
        }
    }


    void LoadRow()
    {
        GameObject rowObjectZero;
        GameObject rowObjectLast;
        GameObject newRow;
        Vector3 posNewRow;
        Vector3 dRow;
        float dOut;

        rowObjectZero = this.transform.GetChild(0).gameObject;
        rowObjectLast = this.transform.GetChild(this.transform.childCount - 1).gameObject;

        if (Screen.height / Screen.width < 2)
        {
            dRow = new Vector3(0f, 300f, 0f);
            dOut = 400f;
        }
        else
        {
            dRow = new Vector3(0f, 200f, 0f);
            dOut = 200f;
        }

        // scroll down
        if (rowObjectZero.transform.position.y < -dOut)
        {
            if (rowObjectLast.GetComponent<rowContentControl>().numRow != 249)
            {
                posNewRow = rowObjectLast.transform.position + dRow;
                if (rowObjectLast.GetComponent<rowContentControl>().kindRow == 0)
                {
                    if (rowObjectLast.GetComponent<rowContentControl>().numRow != 248)
                    {
                        newRow = GetRowFromPool("Odd", posNewRow);
                    }
                    else
                    {
                        newRow = GetRowFromPool("Last", posNewRow);
                    }
                }
                else
                {

                    newRow = GetRowFromPool("Even", posNewRow);
                }

                if (newRow != null)
                {
                    newRow.GetComponent<rowContentControl>().numRow = rowObjectLast.GetComponent<rowContentControl>().numRow + 1;
                }
                PutRowToPool(rowObjectZero);
                SetUpRow(newRow);
            }


        }

        //scroll up
        if (rowObjectLast.transform.position.y > dOut + Screen.height & rowObjectZero.GetComponent<rowContentControl>().numRow != 0)
        {
            posNewRow = rowObjectZero.transform.position - dRow;
            if (rowObjectZero.GetComponent<rowContentControl>().kindRow == 0)
            {
                newRow = GetRowFromPool("Odd", posNewRow);
            }
            else
            {
                newRow = GetRowFromPool("Even", posNewRow);
            }
            if (newRow != null)
            {
                newRow.transform.SetAsFirstSibling();
                newRow.GetComponent<rowContentControl>().numRow = rowObjectZero.GetComponent<rowContentControl>().numRow - 1;

            }

            PutRowToPool(rowObjectLast);
            SetUpRow(newRow);
        }
    }



    void PutRowToPool(GameObject RowToPool)
    {
        if (RowToPool.GetComponent<rowContentControl>().numRow == 249)
        {
            RowToPool.transform.SetParent(Pool.transform.Find("Last"));
        }
        else
        {
            switch (RowToPool.GetComponent<rowContentControl>().kindRow)
            {
                case 0:
                    RowToPool.transform.SetParent(Pool.transform.Find("Even"));
                    break;
                case 1:
                    RowToPool.transform.SetParent(Pool.transform.Find("Odd"));
                    break;
                default:
                    Debug.Log("kindRow error");
                    break;
            }
        }

    }

    GameObject GetRowFromPool(string keyRow, Vector3 RowPosition)
    {
        // kindRow = 0 la last row
        // kindRow = 1 la row le
        // kindRow = 2 la row chan
        GameObject RowFromPool;
        RowFromPool = Pool.transform.Find(keyRow).GetChild(0).gameObject;
        RowFromPool.transform.SetParent(this.transform);
        RowFromPool.transform.position = RowPosition;
        RowFromPool.transform.SetAsLastSibling();
        return RowFromPool;
    }

}
