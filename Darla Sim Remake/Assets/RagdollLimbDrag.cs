using UnityEngine;

public class RagdollLimbDrag : MonoBehaviour
{
    public float dragStrength = 500f; // Strength for dragging the limb smoothly
    private Rigidbody grabbedLimb; // The Rigidbody of the currently grabbed limb
    private ConfigurableJoint joint; // Joint for controlling the grabbed limb
    private Rigidbody[] ragdollRigidbodies; // Array to store all Rigidbody components of the ragdoll
    private JointDrive stiffJointDrive, originalJointDrive; // For stiffening and releasing ragdoll joints
    public float followSpeed = 5f; // Speed for following the player's hand smoothly

    private void Start()
    {
        // Set up joint drive values for stiffening and releasing
        stiffJointDrive = new JointDrive { positionSpring = 1000f, positionDamper = 500f, maximumForce = 1000f };
        originalJointDrive = new JointDrive { positionSpring = 100f, positionDamper = 10f, maximumForce = 1000f };
    }

    // Function to gather all Rigidbody components from a specified GameObject
    public void GatherRigidbodies(GameObject target)
    {
        if (target == null) return;
        ragdollRigidbodies = target.GetComponentsInChildren<Rigidbody>();
    }

    // Function to grab a specific limb
    public void GrabLimb(Rigidbody limb, GameObject parentObj)
    {
        if (limb == null) return;

        GatherRigidbodies(parentObj);
        grabbedLimb = limb;
        grabbedLimb.interpolation = RigidbodyInterpolation.Interpolate; // Enable interpolation for smooth movement

        // Create a ConfigurableJoint to connect the player’s hand to the grabbed limb
        joint = GameGlobals.instance.Player.Hand.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = grabbedLimb;

        // Set joint to smoothly align with hand
        joint.targetPosition = Vector3.zero;
        joint.targetRotation = Quaternion.identity;
        joint.configuredInWorldSpace = false;
        joint.anchor = Vector3.zero;

        // Allow movement with softness to reduce bounciness
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        SoftJointLimitSpring spring = new SoftJointLimitSpring
        {
            spring = 1000f,
            damper = 300f // Increased damper to minimize bouncing
        };
        joint.linearLimitSpring = spring;
        joint.angularXLimitSpring = spring;
        joint.angularYZLimitSpring = spring;
    }

    // Function to release the grabbed limb
    public void ReleaseLimb()
    {
        // Remove the joint from the player’s hand
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        // Restore original settings to all ragdoll joints
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            var configurableJoint = rb.GetComponent<ConfigurableJoint>();
            if (configurableJoint != null)
            {
                configurableJoint.slerpDrive = originalJointDrive;
                configurableJoint.xDrive = originalJointDrive;
                configurableJoint.yDrive = originalJointDrive;
                configurableJoint.zDrive = originalJointDrive;
            }
        }

        if (grabbedLimb != null)
        {
            grabbedLimb.interpolation = RigidbodyInterpolation.None; // Reset interpolation
        }

        grabbedLimb = null;
    }

    private void FixedUpdate()
    {
        // If grabbing a limb, smoothly move it toward the player's hand position
        if (grabbedLimb != null)
        {
            Vector3 targetPosition = GameGlobals.instance.Player.Hand.position;
            Quaternion targetRotation = GameGlobals.instance.Player.Hand.rotation;

            // Smoothly interpolate the position and rotation of the limb
            grabbedLimb.MovePosition(Vector3.Lerp(grabbedLimb.position, targetPosition, followSpeed * Time.fixedDeltaTime));
            grabbedLimb.MoveRotation(Quaternion.Slerp(grabbedLimb.rotation, targetRotation, followSpeed * Time.fixedDeltaTime));
        }
    }
}
