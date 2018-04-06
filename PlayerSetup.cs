using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerSetup : NetworkBehaviour {

	[SyncVar(hook="UpdateColor")]
	public Color m_playerColor;
	
	//make sure the text label only shows up for the local player
	//reserve string variables(public text for player name) 
	//change the prefab so the text element is populating the m_playerNameText field
	//when the client connects: local player will be renamed to base name + m_playerNum
	public string m_basename = "PLAYER";

	[SyncVar(hook="UpdateName")]
	public int m_playerNum = 1;
	public Text m_playerNameText;


	void Start()
	{
		if (!isLocalPlayer)
		{
			UpdateName(m_playerNum);
			UpdateColor(m_playerColor);
		}
	}

//disable the text by default when the client connects - all non-local-player tanks show nothing
	public override void OnStartClient ()
	{
		base.OnStartClient ();
		if (m_playerNameText != null)
		{
			m_playerNameText.enabled = false;
		}
	}

	void UpdateColor (Color pColor)
	{
		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer r in meshes) {
			r.material.color = pColor;
		}
	}

	void UpdateName (int pNum)
	{
		if (m_playerNameText != null) {
			m_playerNameText.enabled = true;
			m_playerNameText.text = m_basename + pNum.ToString ();
		}
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		CmdSetupPlayer();
	}

	[Command]
	void CmdSetupPlayer()
	{
		GameManager.Instance.AddPlayer(this);
		GameManager.Instance.m_playerCount++;
	}


}
