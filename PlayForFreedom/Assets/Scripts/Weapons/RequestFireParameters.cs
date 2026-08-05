using Unity.Netcode;
using UnityEngine;

public struct RequestFireParameters : INetworkSerializable
{
    public Vector3 position;
    public Vector3 direction;
    public float range;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref direction);
        serializer.SerializeValue(ref range);
    }

}
