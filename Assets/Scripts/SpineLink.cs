using UnityEngine;

public class SpineLink : MonoBehaviour
{
    [Header("Spine Physics")]
    public float maxBendAngle = 60f;
    public float snapForce = 20f;   // Lower = more cartoon wobble
    public float damper = 2f;

    [Header("Collapse Settings")]
    [Tooltip("How much strain the joint can take before it breaks and falls.")]
    public float breakingForce = 200f;
    public float breakingTorque = 150f;

    public float weaknessPerBox = 2f;
    
    private bool isAttached = false;
    private ConfigurableJoint myJoint;

    public int stackIndex = 1;

    private void Update()
    {
        if (isAttached && myJoint == null)
        {
            gameObject.tag = "Untagged";
            isAttached = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
            return;
        }

        if (isAttached) return;

        SpineLink hitBox = collision.gameObject.GetComponent<SpineLink>();
        bool hitTray = collision.gameObject.CompareTag("Tray");

        if (hitTray || hitBox != null)
        {
            isAttached = true;
            gameObject.tag = "SpineBox";

            // THE COUNTING LOGIC
            if (hitBox != null)
            {
                stackIndex = hitBox.stackIndex + 1; // I am the bottom box
            }
            else
            {
                stackIndex = 1;
            }

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

        joint.enableCollision = true;

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

        joint.rotationDriveMode = RotationDriveMode.Slerp;
        JointDrive drive = new JointDrive();
        drive.positionSpring = snapForce;
        drive.positionDamper = damper;
        drive.maximumForce = Mathf.Infinity;
        joint.slerpDrive = drive;

        float weakness = 1f + ((stackIndex - 1) * weaknessPerBox);

        joint.breakTorque = breakingTorque / weakness;
        joint.breakForce = breakingForce / weakness;

        Rigidbody myRb = GetComponent<Rigidbody>();
        myRb.solverIterations = 20;
        myRb.solverVelocityIterations = 20;

        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionDistance = 0.1f; // Allowed separation before it snaps back
        joint.projectionAngle = 5f;

        myJoint = joint;
    }

    private void OnJointBreak(float breakForce)
    {
        Debug.Log("SNAP! A joint broke under " + breakForce + " force!");
        isAttached = false;
        gameObject.tag = "Untagged";
    }
}
