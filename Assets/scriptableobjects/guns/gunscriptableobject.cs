// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this gunscriptableobject do?
/// </summary>
[CreateAssetMenu(menuName = "Gun Data")]
public sealed class gunscriptableobject : ScriptableObject
{
    [Range(10 , 100)]
    public int bullets = 10;
    [Range(0 ,5)]
    public float reloadtime = 1.5f;
    [Range(1 , 100)]
    public float damage = 14.5f;
    [Range(0.2f , 3.0f)]
    public float firerate = 2.0f;
    [Range(22,100)]
    public float bulletspeed = 22.0f;
    [Range( 0.0f, -0.3f)]
    public float gunrecoil = -0.05f;
    public GameObject gunprefab;
    [Space(3)]
    public bool ISAutoFire = false;
}
