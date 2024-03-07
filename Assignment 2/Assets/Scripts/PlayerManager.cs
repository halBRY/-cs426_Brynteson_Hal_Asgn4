using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<int> newPlayerId = new NetworkVariable<int>();

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc()
    {
        newPlayerId.Value++;
    }
}
