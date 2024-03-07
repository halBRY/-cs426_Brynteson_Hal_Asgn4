using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
 
public class Bad : NetworkBehaviour
{
    public Score scoreManager;
 
    private void OnCollisionEnter(Collision collision) {
        string team = collision.gameObject.tag;
        scoreManager.LosePointServerRpc(team);
        destroyServerRpc();
}

    [ServerRpc(RequireOwnership = false)]
    void destroyServerRpc()
    {
        NetworkObject.Despawn(gameObject);
    }
}