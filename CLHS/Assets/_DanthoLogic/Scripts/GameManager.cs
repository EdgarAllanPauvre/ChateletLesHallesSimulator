using UnityEngine;
using UnityEngine.SceneManagement;

namespace DanthoLogic
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _main;
        public static GameManager Main => _main;

        [HideInInspector] public bool DEBUG;

        private void Awake()
        {
            if (_main != null) Destroy(this.gameObject);
            _main = this;

            inputs = new InputSystem_Actions();

#if FINAL_GAME_VERSION
            DEBUG = false;
#else
            DEBUG = true;
#endif
        }

        public GameSettings settings;
        public InputSystem_Actions inputs;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}