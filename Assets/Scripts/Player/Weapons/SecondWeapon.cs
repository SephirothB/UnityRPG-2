﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SecondWeapon : MonoBehaviour {
    
	public GameObject Ammo;
    public float Speed = 10f;				// The speed the rocket will fire at.

    public int MaxAmmoCount;
    public int AmmoId;
    
    private int _ammoCount;
    private PlayerControl _playerCtrl;       // Reference to the PlayerControl script.
    private Animator _anim;					// Reference to the Animator component.
    private Text _ammoText;

    // Use this for initialization
    void Start ()
    {
        _ammoText = GameObject.Find("AmmoDisplay").GetComponent<Text>();
        _anim = transform.root.gameObject.GetComponent<Animator>();
        _playerCtrl = transform.root.GetComponent<PlayerControl>();
		if(_playerCtrl.playerStats.Ammo == -1)
			_playerCtrl.playerStats.Ammo = MaxAmmoCount;
        
        UpdateAmmoText();
    }
	
	// Update is called once per frame
	void Update () {
        // If the fire button is pressed...
        if (Input.GetButtonDown(GlobalControl.Instance.KeyboardSettings.Fire2))
        {
            Fire();
        }
    }

    void UpdateAmmoText()
    {
        _ammoText.text = "Ammo: " + _playerCtrl.playerStats.Ammo.ToString();
    }

    private void Fire()
    {
		if(_playerCtrl.playerStats.Ammo > 0)
        {
			_playerCtrl.playerStats.Ammo--;
            UpdateAmmoText();

            // ... set the animator Shoot trigger parameter and play the audioclip.
            _anim.SetTrigger("Shoot2");
            //GetComponent<AudioSource>().Play();

            Vector3 mouseLoc = Input.mousePosition;
            mouseLoc.z = 10.0f;
            mouseLoc = Camera.main.ScreenToWorldPoint(mouseLoc);

            Vector2 dir = DirectionCalc.GetPlayerShotDirection(Vector2.ClampMagnitude(mouseLoc - transform.position, 1.0f));

            // ... instantiate the rocket facing right and set it's velocity to the right. 
            GameObject projectileObject = Instantiate(Ammo);
			Projectile p = projectileObject.GetComponent<Projectile>();

			p.SetLocation(transform.GetChild(0).gameObject.transform.position);
			p.SetDirection(dir);
            p.SetAttack(_playerCtrl.playerStats.BaseAttack + +_playerCtrl.playerStats.WeaponAttack);
        }
        else
        {
            Reload();
        }
    }

    public void Reload()
    {
        if(CheckInventoryForAmmo())
        {
            UseAmmoFromInventory();
			_playerCtrl.playerStats.Ammo = MaxAmmoCount;
            UpdateAmmoText();
        }
    }

    bool CheckInventoryForAmmo()
    {
        Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        return inventory.CheckfItemIsInInventory(AmmoId);
    }

    void UseAmmoFromInventory()
    {
        Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        inventory.UseItem(AmmoId);
    }
}
