using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrnBallDespawn : MonoBehaviour
{
    void Update()
    {
        if (transform.localPosition.magnitude > 5.0f) Destroy(this.gameObject);
    }
}
