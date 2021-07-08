using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "arc-vr/phys/GrabbableObjectType", order = 1)]
public class GrabbableObjectType : ScriptableObject
{
    public enum FollowType { FREE, INTERACTABLE, STATIC, CONSTRAINED, HEAVY };
    public FollowType followType;

    public bool handToObject = false;

    public bool allowTwoHanded = false;
    public bool changeObjectTypeOnTwoHanded;
    public GrabbableObjectType typeOnTwoHanded;

    [Range(0.0f, 1.0f)]
    public float Lightness = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Angular_Lightness = 1.0f;

    [Range(0.05f, 2.0f)]
    public float Break_grab_distance = 0.5f;

    // This multiplier will essentially make the target objects rigidbody weight = weight * 1.0/multiplier
    public float Heavy_force_multiplier = 1.0f;
}