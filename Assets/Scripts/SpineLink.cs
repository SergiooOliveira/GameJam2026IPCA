using UnityEngine;

public class SpineLink : MonoBehaviour
{
    [Header("Spine Physics")]
    public float maxBendAngle = 25f;
    public float snapForce = 60f;
    public float damper = 15f;

    [Header("Collapse Settings")]
    public float breakingForce = 2000f;
    public float breakingTorque = 3000f;
    public float weaknessPerBox = 1.8f;
    public float whipCurve = 1.2f;

    private bool isAttached = false;
    private bool hasBroken = false;
    private ConfigurableJoint myJoint;
    private GameObject lockedTray = null;

    public int stackIndex = 1;

    public string itemType = "Pizza";

    private void Update()
    {
        // Chain reaction: if my anchor disappears, I am falling.
        if (isAttached && myJoint == null)
        {
            isAttached = false;
            hasBroken = true;
            gameObject.tag = "Untagged";

            if (lockedTray != null)
            {
                lockedTray.tag = "Tray";
                lockedTray = null;

                CargoManager cargo = lockedTray.GetComponentInParent<CargoManager>();
                if (cargo != null) cargo.currentItemType = "";
            }
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

        if (Vector3.Angle(transform.up, collision.transform.up) > maxBendAngle) return;

        ContactPoint contact = collision.GetContact(0);
        if (contact.normal.y < 0.8f) return;

        SpineLink hitBox = collision.gameObject.GetComponent<SpineLink>();
        bool hitTray = collision.gameObject.CompareTag("Tray");

        // Prevent broken debris from dragging on the tray
        if (hitTray && hasBroken) return;

        if (hitTray || hitBox != null)
        {
            if (hitTray)
            {
                lockedTray = collision.gameObject;
                lockedTray.tag = "OccupiedTray";

                CargoManager cargo = lockedTray.GetComponentInParent<CargoManager>();
                if (cargo != null) cargo.currentItemType = itemType;
            }

            isAttached = true;
            gameObject.tag = "SpineBox";

            // Climb the stack logic
            stackIndex = (hitBox != null) ? hitBox.stackIndex + 1 : 1;

            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                CreateJoint(hitRb);
            }
        }
    }

    void CreateJoint(Rigidbody connectedBody)
    {
        // We do absolutely NO position math here. 
        // We let the box stay exactly where gravity naturally slammed it.

        Rigidbody myRb = GetComponent<Rigidbody>();
        // Force Unity to calculate this specific box 30 times per frame instead of 6
        myRb.solverIterations = 30;
        myRb.solverVelocityIterations = 30;

        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        joint.enableCollision = true;

        // Lock sliding, allow bending
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = maxBendAngle;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularZLimit = limit;

        float weakness = 1f + ((stackIndex - 1) * weaknessPerBox);
        
        joint.breakTorque = breakingTorque / weakness;
        joint.breakForce = breakingForce / weakness;

        float springWeakness = Mathf.Pow(weakness, whipCurve);

        joint.rotationDriveMode = RotationDriveMode.Slerp;
        JointDrive drive = new JointDrive();
        drive.positionSpring = snapForce / springWeakness;
        drive.positionDamper = damper / springWeakness;
        drive.maximumForce = Mathf.Infinity;
        joint.slerpDrive = drive;

        myJoint = joint;
    }

    private void OnJointBreak(float breakForce)
    {
        isAttached = false;
        hasBroken = true;
        gameObject.tag = "Untagged";

        if (lockedTray != null)
        {
            lockedTray.tag = "Tray";
            lockedTray = null;

            CargoManager cargo = lockedTray.GetComponentInParent<CargoManager>();
            if (cargo != null) cargo.currentItemType = "";
        }
    }
}