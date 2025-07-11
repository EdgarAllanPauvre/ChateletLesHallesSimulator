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

            Application.targetFrameRate = 60;
            inputs = new InputSystem_Actions();

            DontDestroyOnLoad(this.gameObject);

#if FINAL_GAME_VERSION
                        DEBUG = false;
#else
            DEBUG = true;
#endif

#if UNITY_EDITOR
#else
            LoadGame();
#endif

        }

        public GameSettings settings;
        public InputSystem_Actions inputs;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadGame();
            }
        }

        void LoadGame()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
            SceneManager.LoadScene(3, LoadSceneMode.Additive);
        }
    }
}