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
        if (this.transform.localPosition.y < -73285)
        {
            this.transform.localPosition = new Vector3(0f, -73285f, 0f);
        }

        CreateAndDelRow();
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
        DtP.isNewGame=false;
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
        int cMax;
        if (Row.GetComponent<rowContentControl>().numRow == 249)
        {
            cMax = 3;
        }
        else
        {
            cMax = 4;
        }
        for (int c = 0; c < cMax; c++)
        {
            GameObject lvButtonProcessing = Row.transform.Find("LvButtons").GetChild(c).gameObject;
            lvButtonProcessing.GetComponent<LvButton>().level = Row.GetComponent<rowContentControl>().numRow * 4 + c + 1;
            if (lvButtonProcessing.GetComponent<LvButton>().level != 1)
            {
                if (lvButtonProcessing.GetComponent<LvButton>().level <= (int)DtP.lvReached)
                {
                    lvButtonProcessing.GetComponent<LvButton>().isUnlocked = true;
                    lvButtonProcessing.transform.GetChild(0).Find("LvText").gameObject.GetComponent<Text>().text = lvButtonProcessing.GetComponent<LvButton>().level.ToString();
                    lvButtonProcessing.transform.GetChild(0).gameObject.SetActive(true);
                    lvButtonProcessing.transform.GetChild(0).Find("imgTutorial").gameObject.SetActive(false);
                    lvButtonProcessing.transform.GetChild(1).gameObject.SetActive(false);
                    for (byte nStar = 0; nStar < DtP.lvStars[lvButtonProcessing.GetComponent<LvButton>().level - 1]; nStar++)
                    {
                        lvButtonProcessing.transform.GetChild(0).Find("Rate").GetChild(nStar).gameObject.SetActive(true);
                    }

                }
                else
                {
                    lvButtonProcessing.GetComponent<LvButton>().isUnlocked = false;
                    lvButtonProcessing.transform.GetChild(1).Find("LvText").gameObject.GetComponent<Text>().text = lvButtonProcessing.GetComponent<LvButton>().level.ToString();
                    lvButtonProcessing.transform.GetChild(1).gameObject.SetActive(true);
                    lvButtonProcessing.transform.GetChild(0).gameObject.SetActive(false);

                }
            }
            else
            {
                lvButtonProcessing.GetComponent<LvButton>().isUnlocked = true;
                lvButtonProcessing.transform.GetChild(0).Find("LvText").gameObject.SetActive(false);
                lvButtonProcessing.transform.GetChild(0).gameObject.SetActive(true);
                lvButtonProcessing.transform.GetChild(1).gameObject.SetActive(false);
                for (byte nStar = 0; nStar < DtP.lvStars[0]; nStar++)
                {
                    lvButtonProcessing.transform.GetChild(0).Find("Rate").GetChild(nStar).gameObject.SetActive(true);
                }
            }

        }
    }

    void CreateAndDelRow()
    {
        GameObject rowObjectZero;
        GameObject rowObjectLast;
        GameObject newRow;
        Vector3 posNewRow;
        rowObjectZero = this.transform.GetChild(0).gameObject;
        rowObjectLast = this.transform.GetChild(this.transform.childCount - 1).gameObject;
        Vector3 dRow;
        float dOut;
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
        if (rowObjectZero.transform.position.y < -dOut)
        {

            posNewRow = rowObjectLast.transform.position + dRow;
            if (rowObjectLast.GetComponent<rowContentControl>().kindRow == 0)
            {
                if (rowObjectLast.GetComponent<rowContentControl>().numRow != 248)
                {
                    newRow = Instantiate(rowOddContentPf, posNewRow, rot, this.transform);
                }
                else
                {
                    newRow = Instantiate(rowLast, posNewRow, rot, this.transform);
                }
            }
            else
            {
                newRow = Instantiate(rowEvenContentPf, posNewRow, rot, this.transform);
            }

            if (newRow != null)
            {
                newRow.GetComponent<rowContentControl>().numRow = rowObjectLast.GetComponent<rowContentControl>().numRow + 1;
            }

            Destroy(rowObjectZero);
            SetUpRow(newRow);
        }
        rowObjectZero = this.transform.GetChild(0).gameObject;
        rowObjectLast = this.transform.GetChild(this.transform.childCount - 1).gameObject;
        if (rowObjectLast.transform.position.y > dOut + Screen.height & rowObjectZero.GetComponent<rowContentControl>().numRow != 0)
        {
            posNewRow = rowObjectZero.transform.position - dRow;
            if (rowObjectZero.GetComponent<rowContentControl>().kindRow == 0)
            {
                newRow = Instantiate(rowOddContentPf, posNewRow, rot, this.transform);
            }
            else
            {
                newRow = Instantiate(rowEvenContentPf, posNewRow, rot, this.transform);
            }
            if (newRow != null)
            {
                newRow.transform.SetAsFirstSibling();
                newRow.GetComponent<rowContentControl>().numRow = rowObjectZero.GetComponent<rowContentControl>().numRow - 1;

            }

            Destroy(rowObjectLast);
            SetUpRow(newRow);
        }
    }


}
