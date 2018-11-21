using System;
using System.Diagnostics.Eventing.Reader;
using Binding;
using Merch.Data;
using Merch.System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Merch.UI
{
    public class MerchListViewSelection : MonoBehaviour, IPointerClickHandler
    {
        public MerchListView List;
        public MerchGroup Group;
        public GameObject Selection;

        public Material UnavailableMaterial;

        private Material _defaultMaterial;
        private MerchGroup _default;
        private MerchGroup _selected;
        
        [Bind] private Image _image;
        [Bind] private Button _button;

        private bool _enabled = true;

        private void Awake()
        {
            this.Bind();
            _default = List.Group;
            _defaultMaterial = _image.material;
            _selected = List.Group;
            List.OnSetGroup.AddListener(OnGroup);
            MerchSystem.Instance.OnChange += OnItem;
            _button?.onClick.AddListener(Execute);

            if (_image && Group.Properties.Sprite)
                _image.sprite = Group.Properties.Sprite;
        }

        private void OnItem(object sender, EventArgs e)
        {
            var item = MerchSystem.Instance.Selected;
            _enabled = _default != _selected || item == null || MerchSystem.Instance.IsAcquired(item);
            _image.material = _enabled ? _defaultMaterial : UnavailableMaterial;
            if (_button) _button.enabled = _enabled;
        }

        private void OnGroup(MerchGroup arg0)
        {
            _selected = arg0;
            Selection?.SetActive(Group == arg0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Execute();
        }

        private void Execute()
        {
            if (!_enabled) return;
            List.SetGroup(List.Group == Group ? _default : Group);
        }
    }
}