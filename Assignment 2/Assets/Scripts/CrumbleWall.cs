using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleWall : MonoBehaviour
{
    public GameObject debris;
    public MeshRenderer meshRenderer;
    public BoxCollider collider;

    private void OnCollisionEnter(Collision collision) 
    {
        debris.GetComponent<ParticleSystem>().Play();
        meshRenderer.enabled = false;
        collider.enabled = false;
    }

}
