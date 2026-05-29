using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Spawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject itemPrefab;
    public Transform spawnPoint;

    [Tooltip("Drag the Spawner object itself here (needs a Rigidbody!)")]
    public Rigidbody baseTray;

    [Header("Spine Physics")]
    public float maxBendAngle = 30f; // How far the tower can lean before it stops bending
    public float snapForce = 500f;   // How violently it springs back upright
    public float damper = 20f;       // Stops it from jiggling endlessly like jello

    private Rigidbody lastAttachedBody;

    void Start()
    {
        
        // The first block will attach directly to the tray
        lastAttachedBody = baseTray;
    }

    public void OnInteract(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            // 1. Spawn the block
            Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            //Rigidbody newRb = newBlock.GetComponent<Rigidbody>();

            //// 2. Add a springy joint to connect it to the block below it
            //ConfigurableJoint joint = newBlock.AddComponent<ConfigurableJoint>();
            //joint.connectedBody = lastAttachedBody;

            //// 3. Lock positions so they don't slide off each other
            //joint.xMotion = ConfigurableJointMotion.Locked;
            //joint.yMotion = ConfigurableJointMotion.Locked;
            //joint.zMotion = ConfigurableJointMotion.Locked;

            //// 4. Allow angular bending (the wobble!)
            //joint.angularXMotion = ConfigurableJointMotion.Limited;
            //joint.angularYMotion = ConfigurableJointMotion.Locked; // Prevent spinning like a top
            //joint.angularZMotion = ConfigurableJointMotion.Limited;

            //// 5. Set how far it can bend
            //SoftJointLimit limit = new SoftJointLimit();
            //limit.limit = maxBendAngle;
            //joint.lowAngularXLimit = limit;
            //joint.highAngularXLimit = limit;
            //joint.angularZLimit = limit;

            //// 6. Add the Spring (The Snap!)
            //JointDrive drive = new JointDrive();
            //drive.positionSpring = snapForce;
            //drive.positionDamper = damper;
            //drive.maximumForce = Mathf.Infinity;

            //joint.angularXDrive = drive;
            //joint.angularYZDrive = drive;

            //// 7. This new block is now the base for the next one
            //lastAttachedBody = newRb;
        }
    }
}
