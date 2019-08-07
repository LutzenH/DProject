using System;
using System.Collections.Generic;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.UI.Element;
using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.UI
{
    public abstract class AbstractUI
    {
        protected readonly dynamic EntityManager;
        protected readonly dynamic UIManager;
        
        private readonly List<AbstractUIElement> _uiElements;
        
        private Rectangle? _currentRectangleBeingDragged;
        protected (object, string)? PressedButton;
        
        protected AbstractUI(EntityManager entityManager, UIManager uiManager)
        {
            EntityManager = entityManager;
            UIManager = uiManager;

            _uiElements = new List<AbstractUIElement>();
        }

        public virtual void Update(GameTime gameTime)
        {
            var mousePosition = Game1.GetMousePosition();
            PressedButton = null;

            foreach (var element in _uiElements)
            {
                if(element is IUpdateableUIElement updateableUIElement)
                    updateableUIElement.Update(mousePosition, ref _currentRectangleBeingDragged, ref PressedButton);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released && Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                _currentRectangleBeingDragged = null;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in _uiElements)
                element.Draw(spriteBatch);
        }
        
        public void AddUIElement(AbstractUIElement uiElement)
        {
            _uiElements.Add(uiElement);
        }
    }
}
