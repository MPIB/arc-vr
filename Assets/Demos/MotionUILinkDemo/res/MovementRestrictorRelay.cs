using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Motion;

public class MovementRestrictorRelay : MonoBehaviour
{
    public AVR_MovementProvider m_MovementProvider;

    public float max_slope;

    private void Start()
    {
        if(!m_MovementProvider) Destroy(this);
        max_slope = m_MovementProvider.movementRestrictor.validTPmaxSlope;
    }

    private void Update()
    {
        m_MovementProvider.movementRestrictor.validTPmaxSlope = max_slope;
    }
}
