using System.Collections;
using UnityEngine;

public class RagdollLimbDrag : MonoBehaviour
{
    public Transform playerHand; // Reference to the player's hand transform
    public float dragSpeed = 5f; // Speed for dragging
    private Rigidbody grabbedLimb; // The Rigidbody of the currently grabbed limb
    private Rigidbody[] ragdollRigidbodies; // Store all the rigidbodies of the ragdoll
    private bool isDragging = false; // Track whether we're currently dragging

    private void Start()
    {
        playerHand = GameGlobals.instance.Player.Hand; // Reference to the player's hand
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(); // Gather all rigidbodies
    }

    private void Update()
    {
        if (isDragging)
        {
            MoveLimbToHand(grabbedLimb);
        }
    }

    // Function to grab a specific limb
    public void GrabLimb(Rigidbody limb)
    {
        if (limb == null) return;

        grabbedLimb = limb;
        grabbedLimb.isKinematic = false; // Ensure it's not kinematic when grabbing
        isDragging = true; // Set dragging state
        SetRagdollKinematic(false); // Set all rigidbodies to be dynamic
    }

    // Function to release the grabbed limb
    public void ReleaseLimb()
    {
        if (grabbedLimb != null)
        {
            grabbedLimb.isKinematic = false; // Reset kinematic state on release
            grabbedLimb = null; // Clear the reference
            isDragging = false; // Reset dragging state
            SetRagdollKinematic(true); // Set all rigidbodies to kinematic again if desired
        }
    }

    private void MoveLimbToHand(Rigidbody limb)
    {
        Vector3 targetPosition = playerHand.position;
        limb.MovePosition(targetPosition); // Directly move the limb to the player's hand position
        limb.MoveRotation(Quaternion.Slerp(limb.rotation, playerHand.rotation, dragSpeed * Time.fixedDeltaTime)); // Smoothly rotate to match the player's hand rotation
    }

    private void SetRagdollKinematic(bool isKinematic)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = isKinematic; // Set kinematic state for all rigidbodies
        }
    }

    // Additional function to stop dragging
    public void StopDragging()
    {
        ReleaseLimb(); // Release the limb and stop dragging
    }
}
