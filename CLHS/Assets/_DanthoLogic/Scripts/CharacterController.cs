using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DanthoLogic
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Speeds")]
        [SerializeField] float speed0;
        [SerializeField] float speed1;
        [SerializeField] float speedMax;
        [Space]
        [SerializeField] float slowmoSpeed;
        [SerializeField] float speedBoost;
        [Space]
        [SerializeField] float strafeSpeed;

        [Header("Links")]
        [SerializeField] Camera cam;
        [SerializeField] InputSystem_Actions inputActions;
        [SerializeField] InputActionReference strafeInput;

        bool phoneOut;
        float currentSpeed;
        float currentStrafeSpeed;
        Transform t;


        Action<InputAction.CallbackContext> strafeAction;

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            //inputActions.Player.Enable();
            //strafeInput.action.performed += strafeAction = ctx => Strafe(ctx);
            //inputActions.Player.Strafe.performed += strafeAction = ctx => Strafe(ctx);
        }

        private void OnDisable()
        {
            //inputActions.Player.Strafe.performed -= strafeAction;
            //strafeInput.action.performed -= strafeAction = ctx => Strafe(ctx);
            //inputActions.Player.Disable();
        }

        void Init()
        {
            currentSpeed = speed1;
            currentStrafeSpeed = strafeSpeed;
            t = this.transform;
        }

        private void Update()
        {
            t.transform.position += cam.transform.forward * currentSpeed * Time.deltaTime;
            if (strafeInput.action.IsPressed())
            {
                t.transform.position += cam.transform.right * strafeInput.action.ReadValue<float>() * currentStrafeSpeed * Time.deltaTime;
            }
        }

        //void Strafe(InputAction.CallbackContext ctx)
        //{
        //}

        enum ESpeedState
        {
            slomo,
            speed0,
            speed1,
            speedMax,
            speedBoost
        }
    }
}
