using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkNaku.Sound {
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour {
        [SerializeField] private string _soundClipName;
        public Button BTN => _button ??= GetComponent<Button>();

        private Button _button;

        private void OnEnable() {
            BTN.onClick.AddListener(OnClick);
        }

        private void OnDisable() {
            BTN.onClick.RemoveListener(OnClick);
        }

        private void OnClick() {
            Sound.Play(_soundClipName);
        }
    }
}