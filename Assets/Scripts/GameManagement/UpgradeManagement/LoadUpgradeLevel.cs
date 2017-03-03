﻿using UnityEngine;
using System.Collections;

public class LoadUpgradeLevel : MonoBehaviour {
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            GameObject.Find("LevelManager").GetComponent<LevelSave>().SaveLevel();
            Application.LoadLevel("ShipUpgrade");
        }
    }
}
