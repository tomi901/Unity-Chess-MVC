using UnityEngine;
using UnityEngine.SceneManagement;

using Simius;
using Simius.UI;

namespace Chess
{
    public class MainMenu : MenuManager
    {

        [SerializeField, Scene]
        private string gameScene = default;


        public void OnPlayButtonClick()
        {
            SceneManager.LoadScene(gameScene);
        }

        public void OnQuitButtonClick()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }

    }
}