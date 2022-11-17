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
     public gunscriptableobject gunpropertys;

    [Header("Gun Look At Mouse")]

    [SerializeField] private Camera cam;
    [SerializeField] private Transform rotationCenter;
    private Vector3 Pos;
    private Vector3 dir;
    [SerializeField] private float gunaroundplayeradius = 0.75f;

    [Header("gunshootfrompos")]
    [SerializeField] private Transform gunshootfrom;
    private int bullets;
    private bool canshoot = true;
    private bool isreloading = false;
    

    [Header("effects")]
    [SerializeField] private GameObject gunrecoil;
    [SerializeField] private GameObject chamberecoil;
    [SerializeField] private ParticleSystem muzzleflash;

    [Header("mechanics")]
    [SerializeField] private Transform coinspawnpoint;
    [SerializeField] private Transform InstantiateCoin;
    [HideInInspector]
    public int coinamount;
    private void Start()
    {
        bullets = gunpropertys.bullets;
        coinamount = gunpropertys.coinsallowed;
    }
    private void FixedUpdate()
    {
        // change the direction of the gun when its on oppsite side
        gunrecoil.transform.localRotation = transform.localPosition.x <= 0 ? Quaternion.Euler(-180, gunrecoil.transform.localRotation.y, gunrecoil.transform.localRotation.z) : gunrecoil.transform.localRotation = Quaternion.Euler(0, gunrecoil.transform.localRotation.y, gunrecoil.transform.localRotation.z);
        // circluar postion of the gun
        Pos = Input.mousePosition;
        Pos.z = rotationCenter.position.z - cam.transform.position.z;
        Pos = cam.ScreenToWorldPoint(Pos);
        dir = Pos - rotationCenter.position;
        dir = Vector3.ClampMagnitude(dir, gunaroundplayeradius);
        // clamps it to one spot and doesnt allow it to go inside
        if (dir.magnitude >= gunaroundplayeradius)
        {

            transform.position = rotationCenter.position + dir;
            var dirr = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
            var anglerot = Mathf.Atan2(dirr.y, dirr.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(anglerot, Vector3.forward);

        }
    }
    private void Update()
    {
        // check if its autofire or not
        if (gunpropertys.ISAutoFire && Input.GetKey(KeyCode.Mouse0) && canshoot && bullets > 0)
        {
            StartCoroutine(shootgun());
            StartCoroutine(shooteffect());
        }
        else if (!gunpropertys.ISAutoFire && Input.GetKeyDown(KeyCode.Mouse0) && canshoot && bullets > 0)
        {
            StartCoroutine(shootgun());
            StartCoroutine(shooteffect());
        }
        // reload
        if (Input.GetKeyDown(KeyCode.R) && !isreloading)
        {
            StartCoroutine(reload());
        }
        if (Input.GetKeyDown(KeyCode.F) && coinamount > 0)
        {
            Instantiate(InstantiateCoin, coinspawnpoint.position, Quaternion.identity);
            coinamount -= 1;
        }
    }
    private IEnumerator reload()
    {
        isreloading = true;
        yield return new WaitForSeconds(gunpropertys.reloadtime);
        bullets = gunpropertys.bullets;
    }
    private IEnumerator shooteffect()
    {
        LeanTween.moveLocalX(chamberecoil, -0.1f, 0.3f).setEaseOutQuint();

        yield return new WaitForSeconds(0.1f);

        LeanTween.cancel(chamberecoil);
        LeanTween.moveLocalX(chamberecoil, 0.0f, 0.1f).setEaseOutQuint();
    }
    private IEnumerator shootgun()
    {

        canshoot = false;
        bullets -= 1;
        muzzleflash.Play();
        LeanTween.moveLocalX(gunrecoil, gunpropertys.gunrecoil, 0.1f).setEaseOutQuint();
        yield return new WaitForSeconds(0.1f);
        LeanTween.cancel(gunrecoil);
        LeanTween.moveLocalX(gunrecoil, 0.0f, 0.3f).setEaseOutQuint();
        GameObject bullet = Instantiate(gunpropertys.gunprefab, gunshootfrom.position, gunshootfrom.rotation);
        Destroy(bullet, 3.0f);

        yield return new WaitForSeconds(gunpropertys.firerate);

        canshoot = true;
    }
}