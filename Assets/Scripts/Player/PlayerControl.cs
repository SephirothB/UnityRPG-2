﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class customClass
{
    public int customInt;
    public string customString;
}

public class PlayerControl : MonoBehaviour
{
    public Class PlayerClass;

	public bool facingRight = true;			// For determining which way the player is currently facing.
    public Vector2 movement;
    public float RepeatDamagePeriod = 1f;       // How frequently the player can be damaged.

    public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.

	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Animator anim;					// Reference to the player's animator component.
    private Transform t;
    private Rigidbody2D r;
    
    public int HP;
    public int Shield;
    public PlayerStats playerStats;
	private PlayerHealth _playerHealth;
	private PlayerShield _playerShield;
    private PlayerEnergy _playerEnergy;

    public void SavePlayerData()
    {
        GlobalControl.Instance.PlayerStats = playerStats;
    }

    public void LoadPlayerData()
    {
        playerStats = GlobalControl.Instance.PlayerStats;
    }

	void Awake()
	{
		// Setting up references.
		anim = GetComponent<Animator>();
        t = GetComponent<Transform>();
        r = GetComponent<Rigidbody2D>();
		_playerHealth = GetComponent<PlayerHealth> ();
		_playerShield = GetComponent<PlayerShield> ();
        _playerEnergy = GetComponent<PlayerEnergy> ();
    }

	void Start()
	{
		LoadPlayerData ();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "ItemDrop") {
			col.gameObject.GetComponent<ItemDrop>().Add();
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		// If the colliding gameobject is an Enemy...
		if (col.gameObject.tag == "Enemy" && Time.time > playerStats.LastHitTime + RepeatDamagePeriod)
		{
            int enemyAttack = col.gameObject.GetComponent<Enemy>().Attack;
			Hurt(enemyAttack);
		}
	}

	public void Hurt(int Attack)
	{
		playerStats.LastHitTime = Time.time;
		if (playerStats.Shield > 0)
        {
			int extraDamage = _playerShield.DamageShield (Attack);
            if (extraDamage != 0)
                _playerHealth.TakeDamageValue(extraDamage);
		}
        else
        {
			_playerHealth.TakeDamage (Attack);
		}
        if(playerStats.Health == 0)
        {
			Kill ();
		}
	}

	void Kill()
	{

	}

    public void UseEnergy(int Energy)
    {
        _playerEnergy.UseEnergy(Energy);
    }

	void FixedUpdate ()
	{
        HP = playerStats.Health;
        Shield = playerStats.Shield;
		movement = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));        //Put them in a Vector2 Variable (x,y)
        r.velocity = movement * maxSpeed; 		//Add Velocity to the player ship rigidbody

        //Lock the position in the screen by putting a boundaries
        /*GetComponent<Rigidbody2D>().position = new Vector2 
		(
			Mathf.Clamp (GetComponent<Rigidbody2D>().position.x, boundary.xMin, boundary.xMax),  //X
			Mathf.Clamp (GetComponent<Rigidbody2D>().position.y, boundary.yMin, boundary.yMax)	 //Y
		);*/

		// Logic to store facingRight variable.  only needs set on change
		if(movement.x > 0 && !facingRight)
			facingRight = true;
		else if (movement.x < 0 && facingRight)
			facingRight = false;

		if (!facingRight)
			transform.localScale = new Vector3 (-1f, 1f, 1f);
		else
			transform.localScale = new Vector3 (1f, 1f, 1f);

        float xDir = movement.x == 0f ? 0f : movement.x > 0f ? 1f : -1f;
        float yDir = movement.y == 0f ? 0f : movement.y > 0f ? 1f : -1f;
        if (xDir == 0f && yDir == 0f)
            xDir = facingRight ? 1f : -1f;
        anim.SetFloat("MoveX", xDir);
        anim.SetFloat("MoveY", yDir);
    }

    public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!GetComponent<AudioSource>().isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				GetComponent<AudioSource>().clip = taunts[tauntIndex];
				//GetComponent<AudioSource>().Play();
			}
		}
	}

	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
}
