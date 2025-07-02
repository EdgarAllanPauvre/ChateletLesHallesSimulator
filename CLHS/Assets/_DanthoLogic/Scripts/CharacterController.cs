using DanthoLogic.UI;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DanthoLogic
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Speeds")]
        [SerializeField] SpeedParam speed0;
        [SerializeField] SpeedParam speed1;
        [SerializeField] SpeedParam speedMax;
        [Space]
        [SerializeField] SpeedParam slowmoSpeed;
        [SerializeField] SpeedParam speedBoost;
        [Space]
        [SerializeField] float strafeSpeed;
        [SerializeField] float strafeSpeedDuringSpeedBoost;
        [SerializeField] float transiDurationAfterCollisionDuringSpeedBoost;
        [SerializeField] float rotationDuration;

        [Header("Punch")]
        [SerializeField] float hitRange;

        [Header("Links")]
        [SerializeField] Camera cam;
        [SerializeField] InputSystem_Actions inputActions;
        [SerializeField] GameObject phone;
        [SerializeField] Animator animator;

        [Header("Input")]
        [SerializeField] InputActionReference strafeInput;
        [SerializeField] InputActionReference speedBoostInput;
        [SerializeField] InputActionReference slomoInput;
        [SerializeField] InputActionReference lookAtPhoneInput;
        [SerializeField] InputActionReference punchInput;

        bool slomoOn;

        bool phoneOut;
        float currentSpeed;
        float currentStrafeSpeed;
        Transform t;
        SpeedParam currentSpeedParam;
        //ESpeedState currentSpeedState;
        SpeedParam speedBeforeSlomo;
        bool collisionDuringBoost;



        private void Start()
        {
            Init();
            UpdateSpeed(speedMax);
        }

        private void OnEnable()
        {
            lookAtPhoneInput.action.performed += ctx => LookAtPhone();
            speedBoostInput.action.performed += ctx => SpeedBoost();
            punchInput.action.performed += ctx => Punch();
            slomoInput.action.performed += ctx => Slomo();
        }

        private void OnDisable()
        {
            lookAtPhoneInput.action.performed -= ctx => LookAtPhone();
            speedBoostInput.action.performed -= ctx => SpeedBoost();
            punchInput.action.performed -= ctx => Punch();
            slomoInput.action.performed -= ctx => Slomo();
        }

        void Init()
        {
            currentSpeedParam = speed1;
            //currentSpeedState = ESpeedState.speed1;
            currentSpeed = speed1.speed;
            currentStrafeSpeed = strafeSpeed;
            t = this.transform;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            t.transform.position += t.forward * currentSpeed * Time.deltaTime;
            if (strafeInput.action.IsPressed())
            {
                t.transform.position += t.right * strafeInput.action.ReadValue<float>() * currentStrafeSpeed * Time.deltaTime;
            }

            GameUI.Main.UpdateUI(currentSpeedParam.speedState.ToString(), currentSpeed);
        }

        Tween speedTween;
        void UpdateSpeed(SpeedParam newSpeed)
        {
            currentStrafeSpeed = strafeSpeed;

            float targetValue = newSpeed.speed;
            float duration = newSpeed.transitionDuration;

            switch (newSpeed.speedState)
            {
                case ESpeedState.slomo:
                    break;
                case ESpeedState.speed0:
                    break;
                case ESpeedState.speed1:
                    if (collisionDuringBoost)
                    {
                        duration = transiDurationAfterCollisionDuringSpeedBoost;
                        collisionDuringBoost = false;
                    }
                    break;
                case ESpeedState.speedMax:
                    if (currentSpeedParam.speedState != ESpeedState.speed1) return;
                    break;
                case ESpeedState.speedBoost:
                    if (currentSpeedParam.speedState != ESpeedState.speedMax) return;
                    currentStrafeSpeed = strafeSpeedDuringSpeedBoost;
                    break;
                default:
                    break;
            }

            if (speedTween != null && speedTween.IsPlaying()) speedTween.Kill();

            speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, targetValue, duration)
                .OnComplete(() =>
                {
                    currentSpeedParam = newSpeed;
                    //currentSpeedState = newSpeed.speedState;
                    if (currentSpeedParam.speedState == ESpeedState.speed0) UpdateSpeed(speed1);
                    if (currentSpeedParam.speedState == ESpeedState.speed1) UpdateSpeed(speedMax);
                });
        }

        void LookAtPhone()
        {
            phoneOut = !phoneOut;
            phone.SetActive(phoneOut);
        }

        void Punch()
        {
            animator.SetTrigger("punch");
            UpdateSpeed(speed1);
            RaycastHit[] hits = Physics.RaycastAll(t.position, t.forward, hitRange);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<Citizen>() != null)
                {
                    Destroy(hits[i].transform.gameObject);
                }
            }

        }

        void SpeedBoost()
        {
            if (currentSpeedParam.speedState == ESpeedState.speedMax)
                UpdateSpeed(speedBoost);
        }

        void Slomo()
        {
            //if(ctx.performed)

            slomoOn = !slomoOn;

            if (slomoOn)
            {
                speedBeforeSlomo = currentSpeedParam;
                UpdateSpeed(slowmoSpeed);
            }
            else
                UpdateSpeed(speedBeforeSlomo);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<SpeedBreaker>() != null)
            {
                if (currentSpeedParam.speedState == ESpeedState.speedBoost) collisionDuringBoost = true;
                UpdateSpeed(speed0);
            }

            if (other.TryGetComponent<CharacterTurner>(out CharacterTurner ct))
            {
                t.DORotate(ct.newForward.eulerAngles, rotationDuration).OnComplete(() => { Debug.Log(t.forward); });
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red);
        }

        public enum ESpeedState
        {
            slomo,
            speed0,
            speed1,
            speedMax,
            speedBoost
        }

        [Serializable]
        public struct SpeedParam
        {
            public ESpeedState speedState;
            public float speed;
            public float transitionDuration;
        }
    }
}
