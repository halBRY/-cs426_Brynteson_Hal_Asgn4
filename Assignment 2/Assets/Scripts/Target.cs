using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
 
public class Target : NetworkBehaviour
{
    public Score scoreManager;
 
    private void OnCollisionEnter(Collision collision) {
        string team = collision.gameObject.tag;
        scoreManager.AddPointServerRpc(team);
        Destroy(gameObject);

        destroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void destroyServerRpc()
    {
        NetworkObject.Despawn(gameObject);
    }
}