using DanthoLogic.UI;
using DG.Tweening;
using System;

//using UnityEditor;
using UnityEngine;

namespace DanthoLogic
{
    public class CharacterController : MonoBehaviour
    {

        [Header("Links")]
        [SerializeField] Camera cam;
        [SerializeField] InputSystem_Actions inputActions;
        [SerializeField] GameObject phone;
        [SerializeField] Animator animator;
        [SerializeField] new Rigidbody rigidbody;

        GameSettings.PlayerSettings settings;

        bool slomoON;
        bool speedBoostON;

        bool phoneOut;
        float currentSpeed;
        float currentStrafeSpeed;
        Transform t;
        SpeedParam currentSpeedParam;
        SpeedParam speedBeforeSlomo;
        bool collisionDuringBoost;



        private void Start()
        {
            Init();
            UpdateSpeed(settings.speedMax);
        }

        private void OnEnable()
        {
            GameManager.Main.inputs.Player.Enable();
            GameManager.Main.inputs.Player.LookAtPhone.performed += ctx => LookAtPhone();
            GameManager.Main.inputs.Player.Boost.performed += ctx => SpeedBoost();
            GameManager.Main.inputs.Player.Punch.performed += ctx => Punch();
            GameManager.Main.inputs.Player.Slomo.performed += ctx => Slomo();
#if FINAL_GAME_VERSION
#else
            GameManager.Main.inputs.Player.DEBUGTurnLeft.performed += ctx => DEBUGTurnLeft();
            GameManager.Main.inputs.Player.DEBUGTurnRight.performed += ctx => DEBUGTurnRight();
#endif
        }

        private void OnDisable()
        {
#if FINAL_GAME_VERSION
#else
            GameManager.Main.inputs.Player.DEBUGTurnLeft.performed -= ctx => DEBUGTurnLeft();
            GameManager.Main.inputs.Player.DEBUGTurnRight.performed -= ctx => DEBUGTurnRight();
#endif
            GameManager.Main.inputs.Player.LookAtPhone.performed -= ctx => LookAtPhone();
            GameManager.Main.inputs.Player.Boost.performed -= ctx => SpeedBoost();
            GameManager.Main.inputs.Player.Punch.performed -= ctx => Punch();
            GameManager.Main.inputs.Player.Slomo.performed -= ctx => Slomo();
            GameManager.Main.inputs.Player.Disable();
        }

        void Init()
        {
            settings = GameManager.Main.settings.PlayerStng;
            currentSpeedParam = settings.speed1;
            currentSpeed = settings.speed1.speed;
            currentStrafeSpeed = settings.strafeSpeed;
            t = this.transform;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            GameUI.Main.UpdateUI(currentSpeedParam.speedState.ToString(), currentSpeed);
        }

        private void FixedUpdate()
        {
            rigidbody.linearVelocity = t.forward * currentSpeed + (Vector3.down);
            if (GameManager.Main.inputs.Player.Strafe.IsPressed())
            {
                rigidbody.linearVelocity += t.right * GameManager.Main.inputs.Player.Strafe.ReadValue<float>() * currentStrafeSpeed;
            }
        }

        Tween speedTween;
        void UpdateSpeed(SpeedParam newSpeed)
        {
            currentStrafeSpeed = settings.strafeSpeed;

            float targetValue = newSpeed.speed;
            float duration = newSpeed.transitionDuration;

            //OLD SPEED
            switch (currentSpeedParam.speedState)
            {
                case ESpeedState.slomo:
                    duration = settings.slowmoSpeed.transitionDuration;
                    break;
                case ESpeedState.speed0:
                    break;
                case ESpeedState.speed1:
                    break;
                case ESpeedState.speedMax:
                    break;
                case ESpeedState.speedBoost:
                    break;
                default:
                    break;
            }


            //NEW SPEED
            switch (newSpeed.speedState)
            {
                case ESpeedState.slomo:
                    break;
                case ESpeedState.speed0:
                    break;
                case ESpeedState.speed1:
                    if (collisionDuringBoost)
                    {
                        duration = settings.transiDurationAfterCollisionDuringSpeedBoost;
                        collisionDuringBoost = false;
                    }
                    break;
                case ESpeedState.speedMax:
                    if (currentSpeedParam.speedState != ESpeedState.speed1) return;
                    break;
                case ESpeedState.speedBoost:
                    if (currentSpeedParam.speedState != ESpeedState.speedMax) return;
                    currentStrafeSpeed = settings.strafeSpeedDuringSpeedBoost;
                    break;
                default:
                    break;
            }

            if (speedTween != null && speedTween.IsPlaying()) speedTween.Kill();

            speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, targetValue, duration)
                .OnComplete(() =>
                {
                    currentSpeedParam = newSpeed;
                    if (currentSpeedParam.speedState == ESpeedState.speed0) UpdateSpeed(settings.speed1);
                    if (currentSpeedParam.speedState == ESpeedState.speed1) UpdateSpeed(settings.speedMax);
                });
        }

        void LookAtPhone()
        {
            phoneOut = !phoneOut;
            phone.SetActive(phoneOut);
        }

        void Punch()
        {
            if (slomoON) return;

            animator.SetTrigger("punch");
            UpdateSpeed(settings.speed1);
            RaycastHit[] hits = Physics.RaycastAll(t.position, t.forward, settings.hitRange);
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
            speedBoostON = !speedBoostON;

            if (speedBoostON && currentSpeedParam.speedState == ESpeedState.speedMax)
                UpdateSpeed(settings.speedBoost);
            else if (!speedBoostON && (currentSpeedParam.speedState == ESpeedState.speedMax || currentSpeedParam.speedState == ESpeedState.speedBoost))
                UpdateSpeed(settings.speedMax);
        }

        void Slomo()
        {
            slomoON = !slomoON;

            //Wait for last speed to be recovered before being able to go back to slomo
            if (slomoON && currentSpeedParam.speedState == ESpeedState.slomo)
            {
                slomoON = false;
                return;
            }

            if (slomoON)
            {
                speedBeforeSlomo = currentSpeedParam;
                UpdateSpeed(settings.slowmoSpeed);
            }
            else
            {
                UpdateSpeed(speedBeforeSlomo);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<SpeedBreaker>() != null)
            {
                if (currentSpeedParam.speedState == ESpeedState.speedBoost) collisionDuringBoost = true;
                UpdateSpeed(settings.speed0);
            }
        }

        Tween debugRotateTween = null;
        void DEBUGTurnLeft()
        {
            if (debugRotateTween != null && debugRotateTween.IsPlaying()) return;
            debugRotateTween = t.DORotate(t.eulerAngles + new Vector3(0, -90, 0), GameManager.Main.settings.PlayerStng.rotationDuration).SetUpdate(UpdateType.Fixed);

        }
        void DEBUGTurnRight()
        {
            if (debugRotateTween != null && debugRotateTween.IsPlaying()) return;
            debugRotateTween = t.DORotate(t.eulerAngles + new Vector3(0, 90, 0), GameManager.Main.settings.PlayerStng.rotationDuration).SetUpdate(UpdateType.Fixed);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red);
        }
#endif

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
