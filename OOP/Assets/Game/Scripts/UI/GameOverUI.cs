using System;
using Game.Controllers;
using Modules.UI;
using UnityEngine;

namespace Game.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] 
        private GameController gameController;

        [SerializeField] 
        private GameOverView gameOverView;
        
        private void OnEnable()
        {
            gameController.OnGameOver += OnGameOver;
        }
        
        private void OnDisable()
        {
            gameController.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            gameOverView.Show();
        }
    }
}