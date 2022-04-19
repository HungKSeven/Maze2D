using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStorage
{
    public short lvReached;

    public short numStars;

    public int numHint;

    public float yScrollMap;

    public bool isNewGame;

    public byte[] lvStars = new byte[999]; 

    public DataStorage(DataProcessing DtP)
    {
        lvReached = DtP.lvReached;
        numStars = DtP.numStars;
        lvStars = DtP.lvStars; 
        numHint = DtP.numHint;
        isNewGame = DtP.isNewGame;
        yScrollMap = DtP.yScrollMap;

    }
    
}
