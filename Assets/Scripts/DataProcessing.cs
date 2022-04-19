using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProcessing : MonoBehaviour
{

    public static DataProcessing instance;
    public short lvReached;
    public short numStars;
    public int numHint;

    public float yScrollMap;

    public bool isNewGame;

    public int currentLV;

    public byte[] lvStars=new byte[999]; 

    private void Awake() 
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);    
        lvStars=new byte[999];
        isNewGame=true;
        yScrollMap=0;
    }

    

    public void SaveGame()
    {
        DataSystem.SaveData(this);
    }

    public void ResetGame()
    {
        lvReached=1;
        numStars=0;
        numHint=1;
        yScrollMap=0f;
        isNewGame=true;
        lvStars=new byte[999];
        SaveGame();
    }

    public void RandomStart()
    {
        lvReached = (short) Random.Range(2,1000); 
        numHint=lvReached;
        numStars=0;
        for (short i=0;i< lvReached;i++)
        {
            lvStars[i] = (byte) Random.Range(0,4);
            numStars+=lvStars[i];
        }
        SaveGame(); 
    }

    public void LoadGame()
    {
        DataStorage DtS = DataSystem.LoadData();
        if(DtS!=null)
        {
            lvReached = DtS.lvReached;
            numStars = DtS.numStars;
            lvStars = DtS.lvStars;
            numHint = DtS.numHint;
            isNewGame=DtS.isNewGame;
            yScrollMap=DtS.yScrollMap;
        }
        
    }


}
