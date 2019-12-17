using System;
using DProject.Manager;
using Microsoft.Xna.Framework;

namespace DProject.Game
{
    public class GraphicsSettings
    {
        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly Game1 _game;
        
        private bool _enableSSAO;
        private bool _enableLights;
        private int _maxFps;
        private bool _enableVSync;

        public bool EnableFXAA { get; set; }
        public bool EnableSky { get; set; }

        public GraphicsSettings(Game1 game, GraphicsDeviceManager graphicsDeviceManager, ShaderManager shaderManager)
        {
            _game = game;
            _graphicsDeviceManager = graphicsDeviceManager;
            _shaderManager = shaderManager;

            _enableSSAO = true;
            _enableLights = true;
            
            EnableFXAA = true;
            EnableSky = true;
            EnableVSync = false;
            EnableMaxFps = true;
            MaxFps = 120;
        }

        public bool EnableVSync
        {
            get => _graphicsDeviceManager.SynchronizeWithVerticalRetrace;
            set => _graphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
        }
        
        public bool EnableMaxFps
        {
            get => _game.IsFixedTimeStep;
            set => _game.IsFixedTimeStep = value;
        }
        
        public int MaxFps
        {
            get => _maxFps;
            set
            {
                _maxFps = value;
                _game.TargetElapsedTime = TimeSpan.FromTicks(10000000L / MaxFps);
            }
        }
        
        public bool EnableSSAO
        {
            get => _enableSSAO;
            set
            {
                if (_enableSSAO == value)
                    return;
                
                _enableSSAO = value;
                UpdateCombineFinalEffectTechnique();
            }
        }
        
        public bool EnableLights
        {
            get => _enableLights;
            set
            {
                if (_enableLights == value)
                    return;
                
                _enableLights = value;
                UpdateCombineFinalEffectTechnique();
            }
        }
        
        public void UpdateCombineFinalEffectTechnique()
        {
            if (_enableLights && _enableSSAO)
                _shaderManager.CombineFinalEffect.CurrentTechnique = _shaderManager.CombineFinalEffect.Techniques["CombineFinal"];
            else if (_enableSSAO)
                _shaderManager.CombineFinalEffect.CurrentTechnique = _shaderManager.CombineFinalEffect.Techniques["CombineFinalNoLights"];
            else if (_enableLights)
                _shaderManager.CombineFinalEffect.CurrentTechnique = _shaderManager.CombineFinalEffect.Techniques["CombineFinalNoSSAO"];
            else
                _shaderManager.CombineFinalEffect.CurrentTechnique = _shaderManager.CombineFinalEffect.Techniques["CombineFinalNoLightsNoSSAO"];
        }
    }
}
