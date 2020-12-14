using System;
using UnityEngine;


namespace GemPlay.Modules.UIUX.Mobile
{
    public static class Console
    {
        private static readonly string gameObjectName = "Lunar Mobile Console";

        public static void Setup()
        {
            var gameObject = GameObject.Find(gameObjectName);

            if (gameObject == null)
            {
                gameObject = new GameObject(gameObjectName); // LunarConsole.cs assumes that the component is attached to a root game object

                gameObject.AddComponent<LunarConsolePlugin.LunarConsole>();
            }
        }

        public static void Show()
        {
            LunarConsolePlugin.LunarConsole.Show();
        }

        public static void Hide()
        {
            LunarConsolePlugin.LunarConsole.Show();
        }
    }
}
