// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this enemymanager do?
/// </summary>
public sealed class enemymanager : MonoBehaviour
{
    public static enemymanager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public List<Transform> enemys;
    // Start is called before the first frame update
    private void Start()
    {
        enemys = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            enemys.Add(transform.GetChild(i));
        }
    }
}
