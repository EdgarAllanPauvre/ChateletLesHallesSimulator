using UnityEngine;
using UnityEngine.SceneManagement;

namespace DanthoLogic
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _main;
        public static GameManager Main => _main;

        private void Awake()
        {
            if (_main != null) Destroy(this.gameObject);
            _main = this;
        }

        public GameSettings settings;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}