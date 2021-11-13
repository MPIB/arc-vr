using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Net;
using Unity.Netcode;

public class CustomSpawn : AVR_PlayerSpawn
{
    public NetworkObject VRPrefab;
    public NetworkObject nonVRPrefab;

    protected override NetworkObject getPrefab() => vrEnabled ? VRPrefab : nonVRPrefab;
}
