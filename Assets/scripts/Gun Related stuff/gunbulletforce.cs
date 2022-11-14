// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this gunbulletforce do?
/// </summary>
public sealed class gunbulletforce : MonoBehaviour
{
	[SerializeField] private gunscriptableobject gunproperty;
  [SerializeField]  private Rigidbody2D rig;
  // [SerializeField]	private Transform gundirection;
	// Start is called before the first frame update
	private void Start()
	{
		rig.AddForce(transform.right* gunproperty.bulletspeed, ForceMode2D.Impulse);
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
		Destroy(gameObject);
    }
}
