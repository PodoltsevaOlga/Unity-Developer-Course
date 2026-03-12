using Game.Controllers;
using Modules.UI;
using UnityEngine;

namespace Game.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] 
        private GameController gameController;

        [SerializeField] 
        private ScoreView scoreView;

        private void Start()
        {
            scoreView.SetValue(0);
        }
        
        private void OnEnable()
        {
            gameController.OnScoreChanged += OnScoreChanged;
        }
        
        private void OnDisable()
        {
            gameController.OnScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(int newValue)
        {
            scoreView.SetValue(newValue);
        }
    }
}