using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

using AVR.Core;

public class asdasdaaswrt :
#if AVR_NET
    MonoBehaviour
#else
    MonoBehaviour
#endif
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class NetworkState<C> : INetworkSerializable
{
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref Position);
    }
}
