using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkNaku.Sound {
    [RequireComponent(typeof(Toggle))]
    public class ToggleSound : MonoBehaviour {
        [SerializeField] private string _onClip;
        [SerializeField] private string _offClip;

        public Toggle TG => _toggle ??= GetComponent<Toggle>();

        private Toggle _toggle;

        private void OnEnable() {
            TG.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable() {
            TG.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn) {
            Sound.Play(isOn ? _onClip : _offClip);
        }
    }
}