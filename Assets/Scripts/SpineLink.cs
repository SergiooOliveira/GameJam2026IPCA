using UnityEngine;

public class SpineLink : MonoBehaviour
{
    [Header("Spine Physics")]
    public float maxBendAngle = 45f;
    public float snapForce = 30f;   // Lower = more cartoon wobble
    public float damper = 5f;

    private bool isAttached = false;

    void OnCollisionEnter(Collision collision)
    {
        // If we are already part of the tower, do nothing
        if (isAttached) return;

        // Did we land on the base tray or another box already in the spine?
        if (collision.gameObject.CompareTag("Tray") || collision.gameObject.CompareTag("SpineBox"))
        {
            isAttached = true;

            // Change my tag so the next box knows it can stick to me
            gameObject.tag = "SpineBox";

            // Build the joint instantly upon impact
            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                CreateJoint(hitRb);
            }
        }
    }

    void CreateJoint(Rigidbody connectedBody)
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        // Lock the position exactly where it landed!
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Allow the bending wobble
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = maxBendAngle;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularZLimit = limit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = snapForce;
        drive.positionDamper = damper;
        drive.maximumForce = Mathf.Infinity;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
    }
}
