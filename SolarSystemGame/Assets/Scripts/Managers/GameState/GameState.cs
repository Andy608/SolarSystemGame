using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    //What do we do if we are in the pull up menu?
    //The game can still be running in the pull up menu.
    //This class is responsible for the state of the game. Is it paused or simulating?
    public class GameState : ManagerBase<GameState>
    {
        public delegate void PauseAction();
        public static event PauseAction OnGamePaused;

        public delegate void UnPauseAction();
        public static event UnPauseAction OnGameUnPaused;

        public enum EnumGameState
        {
            RUNNING,
            PAUSED
        }

        private EnumGameState currentGameState;

        public EnumGameState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        public bool IsState(EnumGameState state)
        {
            return currentGameState == state;
        }

        public void RequestPause()
        {
            if (currentGameState == EnumGameState.RUNNING)
            {
                //Time.timeScale = 0.0f;
                currentGameState = EnumGameState.PAUSED;

                if (OnGamePaused != null)
                {
                    OnGamePaused();
                }
            }
        }

        public void RequestUnpause()
        {
            if (currentGameState == EnumGameState.PAUSED)
            {
                //Time.timeScale = 1.0f;
                currentGameState = EnumGameState.RUNNING;

                if (OnGameUnPaused != null)
                {
                    OnGameUnPaused();
                }
            }
        }

        public void TogglePause()
        {
            if (currentGameState == EnumGameState.RUNNING)
            {
                RequestPause();
            }
            else if (currentGameState == EnumGameState.PAUSED)
            {
                RequestUnpause();
            }
        }
    }
}
