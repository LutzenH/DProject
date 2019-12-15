using DProject.Manager;

namespace DProject.Game
{
    public class GraphicsSettings
    {
        private readonly ShaderManager _shaderManager;
        
        private bool _enableFXAA;
        private bool _enableSSAO;
        private bool _enableLights;
        private bool _enableSky;
        
        public GraphicsSettings(ShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
        }

        public bool EnableFXAA
        {
            get => _enableFXAA;
            set => _enableFXAA = value;
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
        
        public bool EnableSky
        {
            get => _enableSky;
            set => _enableSky = value;
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
