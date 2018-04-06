﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour {

	PlayerHealth m_pHealth;
	PlayerMotor m_pMotor;
	public PlayerSetup m_pSetup;
	PlayerShoot m_pShoot;

	Vector3 m_originalPosition;
	NetworkStartPosition[] m_spawnPoints;

	public GameObject m_spawnFx;

    [SyncVar]
	public int m_score;

    void OnDestroy()
    {
        GameManager.m_allPlayers.Remove(this);
    }


	void Start () 
	{
		m_pHealth = GetComponent<PlayerHealth>();
		m_pMotor = GetComponent<PlayerMotor>();
		m_pSetup = GetComponent<PlayerSetup>();
		m_pShoot = GetComponent<PlayerShoot>();
	}

	public override void OnStartLocalPlayer()
	{

		m_spawnPoints = GameObject.FindObjectsOfType<NetworkStartPosition>();

		m_originalPosition = transform.position;
	}
		
	Vector3 GetInput()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		return new Vector3 (h, 0, v);

	}

	void FixedUpdate()
	{
		if (!isLocalPlayer || m_pHealth.m_isDead)
		{
			return;
		}

		Vector3 inputDirection = GetInput();
		m_pMotor.MovePlayer(inputDirection);
	}

	void Update()
	{
		if (!isLocalPlayer || m_pHealth.m_isDead)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			m_pShoot.Shoot();
		}

		Vector3 inputDirection = GetInput();
		if (inputDirection.sqrMagnitude > 0.25f)
		{
			m_pMotor.RotateChassis(inputDirection);
		}

		Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, m_pMotor.m_turret.position.y) - m_pMotor.m_turret.position;
		m_pMotor.RotateTurret(turretDir);

	}

    public void EnableControls()
    {
        m_pMotor.Enable();
        m_pShoot.Enable();
    }

    public void DisableControls()
    {
        m_pMotor.Disable();
        m_pShoot.Disable();

    }
        
	void Respawn()
	{
		StartCoroutine("RespawnRoutine");
	}

	IEnumerator RespawnRoutine()
	{
        SpawnPoint oldSpawn = GetNearestSpawnPoint();

		transform.position = GetRandomSpawnPosition();

        if (oldSpawn != null)
        {
            oldSpawn.m_isOccupied = false;
        }

		m_pMotor.m_rigidbody.velocity = Vector3.zero;

		yield return new WaitForSeconds(3f);
		m_pHealth.Reset();

		if (m_spawnFx)
		{
			GameObject spawnFx = Instantiate(m_spawnFx, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
			Destroy(spawnFx, 3f);
		}

        EnableControls();
	}

    SpawnPoint GetNearestSpawnPoint()
    {
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, 3f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        foreach (Collider c in triggerColliders)
        {
            SpawnPoint spawnPoint = c.GetComponent<SpawnPoint>();
            if (spawnPoint != null)
            {
                return spawnPoint;
            }

        }
        return null;
    }


	Vector3 GetRandomSpawnPosition()
	{
		if (m_spawnPoints != null)
		{
			if (m_spawnPoints.Length > 0)
			{
				bool foundSpawner = false;
				Vector3 newStartPosition = new Vector3();
                float timeOut = Time.time + 2f;

				while (!foundSpawner)
				{
					NetworkStartPosition startPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
					SpawnPoint spawnPoint = startPoint.GetComponent<SpawnPoint>();

					if (!spawnPoint.m_isOccupied)
					{
						foundSpawner = true;
						newStartPosition = startPoint.transform.position;
					}

                    if (Time.time > timeOut)
                    {
                        foundSpawner = true;
                        newStartPosition = m_originalPosition;
                    }
				}

				return newStartPosition;
			}
		}
		return m_originalPosition;

	}



}






