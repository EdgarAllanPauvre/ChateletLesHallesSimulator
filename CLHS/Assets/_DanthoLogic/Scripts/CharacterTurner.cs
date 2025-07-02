using UnityEngine;

namespace DanthoLogic
{
    public class CharacterTurner : MonoBehaviour
    {
        //public bool used;
        public Transform newForward;

        private void OnDrawGizmos()
        {
            Debug.DrawRay(newForward.position, newForward.forward, Color.green);
        }
    }
}