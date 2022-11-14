// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this Gun do?
/// </summary>
public sealed class Gun : MonoBehaviour
{
	[Header("Propertys")]
  [SerializeField]	private gunscriptableobject gunpropertys;
	[Header("Gun Look At Mouse")]
	[SerializeField] private Camera cam;
  [SerializeField] private Transform rotationCenter;
	private Vector3 Pos;
	private Vector3 dir;
    private	float radius = 1.0f;
	[Header("GunStuff")]
	private int bullets;
	private bool canshoot = true;
	[SerializeField] private Transform gunshootfrom;
	private void Start()
	{
		bullets = gunpropertys.bullets;
	}

	private void Update()
	{
		Pos = Input.mousePosition;
		Pos.z = rotationCenter.position.z - cam.transform.position.z;
		Pos = cam.ScreenToWorldPoint(Pos);
		dir = Pos - rotationCenter.position;
		dir = Vector3.ClampMagnitude(dir, radius);
		
		if(dir.magnitude >= radius)
        {
		   
			transform.position = rotationCenter.position + dir ;
		var dirr = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
		var anglerot = Mathf.Atan2(dirr.y, dirr.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(anglerot, Vector3.forward);
        }
		if(Vector2.Distance(transform.position , rotationCenter.position ) > 1f)
        {
			dir = Pos * 10 - rotationCenter.position;
			transform.position = rotationCenter.position + dir;
		}
        if (Input.GetKey(KeyCode.Mouse0) && canshoot)
        {
			StartCoroutine(shootgun());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
			bullets = gunpropertys.bullets;
        }

	}

	private IEnumerator shootgun()
    {
		canshoot = false;
		bullets -= 1;
	  GameObject bullet =  Instantiate(gunpropertys.gunprefab, gunshootfrom.position, gunshootfrom.rotation);
		Destroy(bullet, 3.0f);
		yield return new WaitForSeconds(gunpropertys.firerate);
		canshoot = true;
    }
}

