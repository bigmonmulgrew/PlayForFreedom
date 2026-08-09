using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerConfig : INetworkSerializable, IEquatable<PlayerConfig>
{
    // private PlayerConfig DefaultConfig => new PlayerConfig(IsInitialized = true, "Dave", -10000, Color.white, Color.white, Color.white);

    public bool IsInitialized;

    public FixedString64Bytes name;
    public int startingMoney;
    public Color customColour1;
    public Color customColour2;
    public Color customColour3;

    public PlayerConfig(bool isInitialized, FixedString64Bytes name, int startingMoney, Color customColour1, Color customColour2, Color customColour3)
    {
        this.IsInitialized = isInitialized;
        this.name = name;
        this.startingMoney = startingMoney;
        this.customColour1 = customColour1;
        this.customColour2 = customColour2;
        this.customColour3 = customColour3;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsInitialized);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref startingMoney);
        serializer.SerializeValue(ref customColour1);
        serializer.SerializeValue(ref customColour2);
        serializer.SerializeValue(ref customColour3);
    }

    public bool Equals(PlayerConfig other)
    {
        return IsInitialized == other.IsInitialized
            && name.Equals(other.name)
            && startingMoney.Equals(other.startingMoney)
            && customColour1.Equals(other.customColour1)
            && customColour2.Equals(other.customColour2)
            && customColour3.Equals(other.customColour3);
    }
    
}
