using TMPro;
using UnityEngine;

namespace DanthoLogic.UI
{
    public class GameUI : MonoBehaviour
    {
        private static GameUI _main;
        public static GameUI Main { get { return _main; } }

        [SerializeField] TextMeshProUGUI DEBUG_State;
        [SerializeField] TextMeshProUGUI DEBUG_Speed;
        [SerializeField] TextMeshProUGUI DEBUG_FPS;

        private void Awake()
        {
            _main = this;
        }

        public void UpdateUI(string state, float speed)
        {
            DEBUG_State.text = state;
            DEBUG_Speed.text = speed.ToString();
            DEBUG_FPS.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        }
    }
}