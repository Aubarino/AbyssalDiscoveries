using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    void Awake()
    {
        //setup all the databases
        ItemDatabase.Startup();
    }

}
