using DG.Tweening;
using UnityEngine;

namespace DanthoLogic
{
    public class CharacterTurner : MonoBehaviour
    {
        //public bool used;
        //public Transform newForward;
        public float newForward;
        public float newBackard;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, new Vector3(Mathf.Cos((newForward + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((newForward + 90) * Mathf.Deg2Rad)), Color.blue);
            Debug.DrawRay(transform.position, new Vector3(Mathf.Cos((newBackard + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((newBackard + 90) * Mathf.Deg2Rad)), Color.red);

            var bc = GetComponentInChildren<BoxCollider>();
            Vector3 point1 = transform.position + (bc.size.x * .5f * transform.right);
            Vector3 point2 = transform.position - (bc.size.x * .5f * transform.right);
            Debug.DrawLine(point1, point2, Color.green);
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CharacterController>(out CharacterController cc))
            {
                Debug.Log("entered");
                cc.transform.DORotate(new Vector3(0, -newForward, 0), GameManager.Main.settings.PlayerStng.rotationDuration);
            }
        }
    }
}