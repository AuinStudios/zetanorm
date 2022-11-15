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
    
    public int bullets = 10;
    public float reloadtime = 1.5f;
    public float damage = 14.5f;
    public float firerate = 2.0f;
    public float bulletspeed = 30.0f;
    public float gunrecoil = -0.05f;
    public GameObject gunprefab;
    [Space(3)]
    public bool ISAutoFire = false;
}
