using Game.Controllers;
using Modules.UI;
using UnityEngine;

namespace Game.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] 
        private ScoreCounter scoreCounter;

        [SerializeField] 
        private ScoreView scoreView;

        private void Start()
        {
            scoreView.SetValue(0);
        }
        
        private void OnEnable()
        {
            scoreCounter.OnScoreChanged += OnScoreChanged;
        }
        
        private void OnDisable()
        {
            scoreCounter.OnScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(int newValue)
        {
            scoreView.SetValue(newValue);
        }
    }
}