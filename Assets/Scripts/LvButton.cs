using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvButton : MonoBehaviour
{


    public int level;
    public bool isUnlocked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GotoLV()
    {
       this.transform.parent.parent.parent.GetComponent<ContentControl>().GotoLV(level);
    }

    public void showNotiLvLocked()
    {
        this.transform.parent.parent.parent.GetComponent<ContentControl>().showNotiLvLocked();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
