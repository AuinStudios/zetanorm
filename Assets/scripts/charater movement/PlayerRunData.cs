// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this PlayerRunData do?
/// </summary>
/// 
[CreateAssetMenu(menuName = "Player Data")]
public sealed class PlayerRunData : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed = 6.3f; //Target speed we want the player to reach.
    public float runAcceleration = 0.3f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
    [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDecceleration = 0.19f; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    [Space(10)]
    [Range(0.01f, 1)] public float accelInAir = 0.251f; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] public float deccelInAir = 0.296f;
    public bool doConserveMomentum;


    private void OnValidate()
    {
        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
