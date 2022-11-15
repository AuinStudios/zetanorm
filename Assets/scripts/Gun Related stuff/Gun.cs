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
  [SerializeField]  private	float gunaroundplayeradius = 1.0f;
	[Header("GunStuff")]
	private int bullets;
	private bool canshoot = true;
	[SerializeField] private Transform gunshootfrom;
	[Header("effects")]
	[SerializeField] private GameObject gunrecoil;
	[SerializeField] private GameObject partofgun;
	[SerializeField] private ParticleSystem muzzleflash;
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
		dir = Vector3.ClampMagnitude(dir, gunaroundplayeradius);
		
		if(dir.magnitude >= gunaroundplayeradius)
        {
		  
			transform.position = rotationCenter.position + dir ;
		var dirr = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
		var anglerot = Mathf.Atan2(dirr.y, dirr.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(anglerot, Vector3.forward);
        }
        //if(Vector2.Distance(transform.position , rotationCenter.position ) > 1f)
        //{
        //	dir = Pos * 10 - rotationCenter.position;
        //	transform.position = rotationCenter.position + dir;
        //}
       // if (gunpropertys.ISAutoFire)
       // {
	   //
       // }
        if (gunpropertys.ISAutoFire && Input.GetKey(KeyCode.Mouse0) && canshoot && bullets > 0)
        {
			StartCoroutine(shootgun());
			StartCoroutine(shooteffect());
        }
        else if (gunpropertys.ISAutoFire == false && Input.GetKeyDown(KeyCode.Mouse0) && canshoot && bullets > 0)
        {
			StartCoroutine(shootgun());
			StartCoroutine(shooteffect());
		}
        if (Input.GetKeyDown(KeyCode.R))
        {
			bullets = gunpropertys.bullets;
        }

	}
	private IEnumerator shooteffect()
    {
        LeanTween.moveLocalX(partofgun, -0.1f, 0.3f).setEaseOutQuint();
		yield return new WaitForSeconds(0.1f);
		LeanTween.cancel(partofgun);
		LeanTween.moveLocalX(partofgun, 0.0f, 0.1f).setEaseOutQuint();
	}
	private IEnumerator shootgun()
    {
		
		canshoot = false;
		bullets -= 1;
		muzzleflash.Play();
		LeanTween.moveLocalX(gunrecoil, gunpropertys.gunrecoil, 0.1f).setEaseOutQuint();
		yield return new WaitForSeconds(0.1f);
		LeanTween.cancel(gunrecoil);
		LeanTween.moveLocalX(gunrecoil, 0.0f , 0.3f).setEaseOutQuint();
		GameObject bullet =  Instantiate(gunpropertys.gunprefab, gunshootfrom.position, gunshootfrom.rotation);
		Destroy(bullet, 3.0f);

		yield return new WaitForSeconds(gunpropertys.firerate);
		
		canshoot = true;
    }
}