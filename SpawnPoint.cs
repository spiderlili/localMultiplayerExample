using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnPoint : NetworkBehaviour 
{


	public bool m_isOccupied;


	void OnTriggerEnter (Collider other)
	{
		if ( other.gameObject.tag == "Player")
		{
			m_isOccupied = true;
		}
	}


	void OnTriggerStay (Collider other)
	{
		if ( other.gameObject.tag == "Player")
		{
			m_isOccupied = true;
		}
	}

   
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{
			m_isOccupied = false;
		}
	}


}
