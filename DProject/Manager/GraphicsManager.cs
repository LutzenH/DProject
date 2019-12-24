using System;
using DProject.Manager;

namespace DProject.Game
{
    public sealed class GraphicsManager
    {
        private static GraphicsManager _instance;
        private GraphicsManager()
        {
            EnableFXAA = false;
            EnableSky = true;
            EnableVSync = false;
            EnableFullscreen = false;
            EnableMaxFps = true;
            MaxFps = 120;
            EnableSSAO = false;
            EnableLights = false;
        }
        
        public static GraphicsManager Instance
        {
            get => _instance ?? (_instance = new GraphicsManager());
            set => _instance = value;
        }

        public bool EnableFXAA { get; set; }
        public bool EnableSky { get; set; }
        public bool EnableVSync { get; set; }
        public bool EnableFullscreen { get; set; }
        public bool EnableMaxFps { get; set; }
        public int MaxFps { get; set; }
        public bool EnableSSAO { get; set; }
        public bool EnableLights { get; set; }

        public void UpdateGraphicsSettings(Game1 game)
        {
            game.Graphics.SynchronizeWithVerticalRetrace = EnableVSync;
            game.IsFixedTimeStep = EnableMaxFps;
            game.TargetElapsedTime = TimeSpan.FromTicks(10000000L / MaxFps);
            
            if(game.Graphics.IsFullScreen != EnableFullscreen)
                game.Graphics.ToggleFullScreen();
            
            UpdateCombineFinalEffectTechnique();
        }
        
        private void UpdateCombineFinalEffectTechnique()
        {
            var sm = ShaderManager.Instance;
            
            if (EnableLights && EnableSSAO)
                sm.CombineFinalEffect.CurrentTechnique = sm.CombineFinalEffect.Techniques["CombineFinal"];
            else if (EnableSSAO)
                sm.CombineFinalEffect.CurrentTechnique = sm.CombineFinalEffect.Techniques["CombineFinalNoLights"];
            else if (EnableLights)
                sm.CombineFinalEffect.CurrentTechnique = sm.CombineFinalEffect.Techniques["CombineFinalNoSSAO"];
            else
                sm.CombineFinalEffect.CurrentTechnique = sm.CombineFinalEffect.Techniques["CombineFinalNoLightsNoSSAO"];
        }
    }
}
