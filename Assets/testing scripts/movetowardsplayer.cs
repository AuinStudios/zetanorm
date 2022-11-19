// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this movetowardsplayer do?
/// </summary>
public sealed class movetowardsplayer : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private Rigidbody2D rig;
	// Start is called before the first frame update
	private void Start()
	{
		
	}

	// Update is called once per frame
	private void Update()
	{
		
         rig.AddForce(player.position - transform.position  * 1.3f);
	}
}
