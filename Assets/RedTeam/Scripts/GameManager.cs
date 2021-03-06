﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedTeam {

    /// <summary>
    /// The GameManager is responsible for initializing certain generic systems and loading the first real scene of the game
    /// </summary>
    public class GameManager : MonoBehaviour {

        public string mainMenuScene;
        public TransitionManager transitionManager;

        public EngineData initialEngineData;

        IEngine _currentEngine;

        EngineFactory _engineFactory;

        public void SetCurrentEngine(EngineData data) {

            IEngine newEngine = _engineFactory.GenerateEngine(data);

            if(newEngine == null) {

                Debug.LogErrorFormat("Unsupported engine type: {0}", data.GetType());

            } else {

                StartCoroutine(SwitchEngines(newEngine));
            }
        }

        IEnumerator SwitchEngines(IEngine newEngine) {

            IEngine previousEngine = _currentEngine;

            if (previousEngine != null)
                yield return previousEngine.StopEngine();

            _currentEngine = newEngine;

            yield return newEngine.StartEngine(previousEngine);
        }

        void Start() {

            SetCurrentEngine(initialEngineData);
        }

        void Awake() {
            
            EventManager.Init();

            _engineFactory = new EngineFactory();

            EventManager.AddRequest<GameManager>("GameManager", () => this);
            EventManager.AddRequest<TransitionManager>("TransitionManager", () => transitionManager);
            EventManager.AddRequest<IEngine>("CurrentEngine", () => _currentEngine);

            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy() {

            EventManager.Clean();
        }
    }
}