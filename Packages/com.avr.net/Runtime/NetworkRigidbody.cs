using System;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

/*
THE CODE BELOW IS TAKEN FROM MLAPI-COMMUNITY-CONTRIBUTIONS:

LINK:
https://github.com/Unity-Technologies/mlapi-community-contributions/blob/main/com.mlapi.contrib.extensions/Runtime/NetworkRigidbody/NetworkRigidbody.cs

LICENSE:
MIT License
Copyright (c) 2021 Unity Technologies
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace MLAPI {
    public class NetworkRigidbody : NetworkBehaviour
    {
        public NetworkVariableVector3 netVelocity = new NetworkVariableVector3(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.OwnerOnly });
        public NetworkVariableVector3 netAngularVelocity = new NetworkVariableVector3(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.OwnerOnly });
        public NetworkVariableVector3 netPosition = new NetworkVariableVector3(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.OwnerOnly });
        public NetworkVariableQuaternion netRotation = new NetworkVariableQuaternion(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.OwnerOnly });
        public NetworkVariableUInt netUpdateId = new NetworkVariableUInt(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.OwnerOnly });

        [SerializeField]
        bool m_SyncVelocity = true;

        [SerializeField]
        bool m_SyncAngularVelocity = true;

        [SerializeField]
        bool m_SyncPosition = true;

        [SerializeField]
        bool m_SyncRotation = true;

        [SerializeField]
        float m_InterpolationTime;

        [Serializable]
        struct InterpolationState
        {
            public Vector3 PositionDelta;
            public Quaternion RotationDelta;
            public Vector3 VelocityDelta;
            public Vector3 AngularVelocityDelta;
            public float TimeRemaining;
            public float TotalTime;
        }

        uint m_InterpolationChangeId;
        InterpolationState m_InterpolationState;
        Rigidbody m_Rigidbody;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void BeginInterpolation()
        {
            m_InterpolationState = new InterpolationState()
            {
                PositionDelta = netPosition.Value - m_Rigidbody.position,
                RotationDelta = Quaternion.Inverse(m_Rigidbody.rotation) * netRotation.Value,
                VelocityDelta = netVelocity.Value - m_Rigidbody.velocity,
                AngularVelocityDelta = netAngularVelocity.Value - m_Rigidbody.angularVelocity,
                TimeRemaining = m_InterpolationTime,
                TotalTime = m_InterpolationTime
            };
        }

        void FixedUpdate()
        {
            if (!NetworkVariablesInitialized())
            {
                return;
            }

            if (IsOwner)
            {
                bool changed = false;

                if (m_SyncPosition)
                {
                    changed |= TryUpdate(netPosition, m_Rigidbody.position);
                }

                if (m_SyncRotation)
                {
                    changed |= TryUpdate(netRotation, m_Rigidbody.rotation);
                }

                if (m_SyncVelocity)
                {
                    changed |= TryUpdate(netVelocity, m_Rigidbody.velocity);
                }

                if (m_SyncAngularVelocity)
                {
                    changed |= TryUpdate(netAngularVelocity, m_Rigidbody.angularVelocity);
                }

                if (changed)
                {
                    netUpdateId.Value++;
                }
            }
            else
            {
                if (m_InterpolationChangeId != netUpdateId.Value)
                {
                    BeginInterpolation();
                    m_InterpolationChangeId = netUpdateId.Value;
                }

                float deltaTime = Time.fixedDeltaTime;
                if (0 < m_InterpolationState.TimeRemaining)
                {
                    deltaTime = Mathf.Min(deltaTime, m_InterpolationState.TimeRemaining);
                    m_InterpolationState.TimeRemaining -= deltaTime;

                    deltaTime /= m_InterpolationState.TotalTime;

                    if (m_SyncPosition)
                    {
                        m_Rigidbody.position +=
                            m_InterpolationState.PositionDelta * deltaTime;
                    }

                    if (m_SyncRotation)
                    {
                        m_Rigidbody.rotation =
                            m_Rigidbody.rotation * Quaternion.Slerp(Quaternion.identity, m_InterpolationState.RotationDelta, deltaTime);
                    }

                    if (m_SyncVelocity)
                    {
                        m_Rigidbody.velocity +=
                            m_InterpolationState.VelocityDelta * deltaTime;
                    }

                    if (m_SyncAngularVelocity)
                    {
                        m_Rigidbody.angularVelocity +=
                            m_InterpolationState.AngularVelocityDelta * deltaTime;
                    }
                }
            }
        }

        bool NetworkVariablesInitialized()
        {
            return netVelocity.Settings.WritePermission == NetworkVariablePermission.OwnerOnly;
        }

        static bool TryUpdate(NetworkVariableVector3 variable, Vector3 value)
        {
            var current = variable.Value;
            if (Mathf.Approximately(current.x, value.x)
                && Mathf.Approximately(current.y, value.y)
                && Mathf.Approximately(current.z, value.z))
            {
                return false;
            }

            variable.Value = value;
            return true;
        }

        static bool TryUpdate(NetworkVariableQuaternion variable, Quaternion value)
        {
            var current = variable.Value;
            if (Mathf.Approximately(current.x, value.x)
                && Mathf.Approximately(current.y, value.y)
                && Mathf.Approximately(current.z, value.z)
                && Mathf.Approximately(current.w, value.w))
            {
                return false;
            }

            variable.Value = value;
            return true;
        }
    }
}
