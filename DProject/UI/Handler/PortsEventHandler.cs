using DProject.Entity.Ports;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.UI.Element.Ports;

namespace DProject.UI.Handler
{
    public class PortsEventHandler
    {
        private readonly PortsUI _portsUI;
        private readonly GameTimeEntity _gameTimeEntity;
        
        public PortsEventHandler(EntityManager entityManager, GameUIManager uiManager)
        {
            _gameTimeEntity = entityManager.GetGameTimeEntity();
            _portsUI = uiManager.GetPortsUI();
            
            _gameTimeEntity.TimeChanged += (sender, args) => _portsUI.Clock.SetRotation(args.CurrentTime);
            _gameTimeEntity.SeasonChanged += (season, args) => _portsUI.SeasonIndicator.SetSeason(args.CurrentSeason);
        }

        public void HandleInput(Button button, object[] linkedValues = null)
        {
            switch (button)
            {
                case Button.TimeControlPause:
                    _gameTimeEntity.Paused = true;
                    _portsUI.TimeControl.SelectedButton = TimeControlUIElement.TimeControlOption.Pause;
                    break;
                case Button.TimeControlPlay:
                    _gameTimeEntity.Paused = false;
                    _gameTimeEntity.GameSpeed = 1.0f;
                    _portsUI.TimeControl.SelectedButton = TimeControlUIElement.TimeControlOption.Play;
                    break;
                case Button.TimeControlSpeedUp:
                    _gameTimeEntity.Paused = false;
                    _gameTimeEntity.GameSpeed = 3.0f;
                    _portsUI.TimeControl.SelectedButton = TimeControlUIElement.TimeControlOption.Speedup;
                    break;
                case Button.MapButtonPressed:
                    break;
            }
        }

    }

    public enum Button {
        TimeControlPause, TimeControlPlay, TimeControlSpeedUp, TimeControlEnd, MapButtonPressed
    };
} 
