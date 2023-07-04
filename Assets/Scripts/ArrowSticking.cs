using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ArrowSticking : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider myCollider;
    [SerializeField] private GameObject stickingArrow;
    private XROrigin XROrigin;

    void OnCollisionEnter(Collision collision) {
        rb.isKinematic = true;
        myCollider.isTrigger = true;

        GameObject arrow = Instantiate(stickingArrow);
        arrow.transform.position = transform.position;
        arrow.transform.forward = transform.forward;

        if (collision.collider.attachedRigidbody != null) {
            // This can run booky if we manipulate scale of the parent object
            arrow.transform.parent = collision.collider.attachedRigidbody.transform;
        }

        // collision.collider.GetComponent<IHittable>()?.GetHit();
        XROrigin = FindObjectOfType<XROrigin>();
        XROrigin.GetComponent<PlayerController>().TeleportPlayer(transform.position);

        Destroy(gameObject);
    }

}
