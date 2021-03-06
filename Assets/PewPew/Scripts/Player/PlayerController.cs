﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedTeam.PewPew {

    public class PlayerController : GameEventListener {

        const string ShipSelectKey = "ShipSelect";

        TransitionManager _transitionManager;

        TransitionManager TransitionManager {
            get {
                if (_transitionManager == null)
                    _transitionManager = EventManager.Request<TransitionManager>("TransitionManager");

                return _transitionManager;
            }
        }

        GameController _gameController;

        GameController GameController {
            get {
                if (_gameController == null)
                    _gameController = EventManager.Request<GameController>("GameController");

                return _gameController;
            }
        }

        public float maxSpeed;
        public float shieldRechargeTime;
        public bool wasHit;
        public float gracePeriod = 1f;

        public GameObject[] shipModels = new GameObject[3];

        float timeOfLastHit;

        void OnTriggerEnter(Collider col) {

            if (col.tag == "playerGun" || col.tag == "playerMissile" || col.tag == "EnemyBox")
                return;

            Hit();
        }

        public void Hit() {

            if (Time.time - timeOfLastHit < gracePeriod)
                return;

            timeOfLastHit = Time.time;

            GameController.LoseHeart();
        }

        void Update() {

            if (!_playing || _paused)
                return;

            if (Time.time - timeOfLastHit > shieldRechargeTime && !GameController.ShieldUp)
                GameController.ShieldUp = true;

            float verticalMoveSpeed = maxSpeed * Input.GetAxis("Vertical");
            float horizontalMoveSpeed = maxSpeed * Input.GetAxis("Horizontal");

            if (((transform.localPosition.x < -3.5) && (horizontalMoveSpeed < 0)) || ((transform.localPosition.x > 3.5) && (horizontalMoveSpeed > 0)))
                horizontalMoveSpeed = 0;

            if (((transform.localPosition.y < -2) && (verticalMoveSpeed < 0)) || ((transform.localPosition.y > 2) && (verticalMoveSpeed > 0)))
                verticalMoveSpeed = 0;

            transform.Translate((Vector3.up * verticalMoveSpeed * Time.fixedDeltaTime) + (Vector3.right * horizontalMoveSpeed * Time.fixedDeltaTime));
        }

        void Start() {

            timeOfLastHit = Time.time;
        }

        protected override void OnInitGame() {

            int shipSelect = PlayerPrefs.GetInt(ShipSelectKey, -1);

            // If no prefs exist, something is probably wrong,
            // but for now, just don't do anything to the ships
            // and don't interrupt play mode
            if (shipSelect > 0) {

                foreach (GameObject ship in shipModels)
                    ship.SetActive(false);

                shipModels[shipSelect].SetActive(true);
            }
        }

        protected override void Awake() {

            EventManager.AddRequest<PlayerController>("PlayerController", () => this);

            base.Awake();
        }

        protected override void OnDestroy() {

            EventManager.RemoveRequest<PlayerController>("PlayerController");

            base.Awake();
        }
    }
}