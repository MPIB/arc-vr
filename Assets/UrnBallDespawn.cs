using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrnBallDespawn : MonoBehaviour
{
    void Update()
    {
        if (transform.localPosition.magnitude > 1.5f) Destroy(this.gameObject);
    }
}
