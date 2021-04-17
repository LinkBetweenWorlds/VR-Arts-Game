using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{
    public XRNode leftSource;
    public XRNode rightSource;
    public float speed = 3;
    public float jumpPower = 100;
    public LayerMask groundLayer;
    public float additionHeight = 0.2f;

    //Left Controller Inputs
    private Vector2 leftInputAxis;
    private bool leftSecondaryButton;

    //Right Controller Inputs
    private bool rightPrimaryButton;
    private bool rightSecondaryButton;

    private CharacterController character;
    private XRRig rig;
    private Rigidbody rb;
    private Arts art;
    private InputDevice leftHand;
    private InputDevice rightHand;
    private float verticalSpeed;
    private float gravity = -9.81f;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
        rb = GetComponent<Rigidbody>();
        art = GetComponent<Arts>();

        leftHand = InputDevices.GetDeviceAtXRNode(leftSource);
        rightHand = InputDevices.GetDeviceAtXRNode(rightSource);
    }
    private void Update()
    {
        if (leftHand == null || rightHand == null)
        {
            leftHand = InputDevices.GetDeviceAtXRNode(leftSource);
            rightHand = InputDevices.GetDeviceAtXRNode(rightSource);
        }


    }

    private void FixedUpdate()
    {
        Movement();
        ActivatePowers();
    }

    void Movement()
    {
        CapsuleFollowHeadset();
        bool isGrounded = CheckIfGrounded();

        leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftInputAxis);
        rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButton);

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(leftInputAxis.x, 0, leftInputAxis.y);

        if (art.speedArtActive)
        {
            character.Move(direction * speed * art.speedMultiplier * Time.fixedDeltaTime);
        }
        else
        {
            character.Move(direction * speed * Time.fixedDeltaTime);
        }

        if (isGrounded)
        {
            if (rightPrimaryButton)
            {
                if (art.jumpArtActive)
                {
                    verticalSpeed = jumpPower * art.jumpMultiplier;
                }
                else
                {
                    verticalSpeed = jumpPower;
                }
            }
            else
            {
                verticalSpeed = 0;
            }
        }
        else
        {
            verticalSpeed += gravity * Time.fixedDeltaTime;
        }

        character.Move(Vector3.up * verticalSpeed * Time.fixedDeltaTime);
    }

    void CapsuleFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight + additionHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, ((character.height / 2) + character.skinWidth), capsuleCenter.z);
    }

    void ActivatePowers()
    {
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out rightSecondaryButton);
        leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out leftSecondaryButton);

        if (art.artCooldown == false)
        {
            if (art.artActive == false)
            {
                if (rightSecondaryButton)
                {
                    art.speedArtActive = true;
                }
                else if (leftSecondaryButton)
                {
                    art.jumpArtActive = true;
                }
            }
        }
    }

    bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
