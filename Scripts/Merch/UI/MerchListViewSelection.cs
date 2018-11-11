using Binding;
using Merch.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Merch.UI
{
    [ExecuteInEditMode]
    public class MerchListViewSelection : MonoBehaviour, IPointerClickHandler
    {
        public MerchListView List;
        public MerchGroup Group;
        public GameObject Selection;

        private MerchGroup _default;
        
        [Bind]
        private Image _image;

        private void Awake()
        {
            this.Bind();
            _default = List.Group;
            List.OnSetGroup.AddListener(OnGroup);
            var btn = GetComponent<Button>();
            btn?.onClick.AddListener(Execute);
            
            if (_image && Group.Properties.Sprite.Enabled)
                _image.sprite = Group.Properties.Sprite.Sprite;
        }

        private void OnGroup(MerchGroup arg0)
        {
            Selection?.SetActive(Group == arg0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Execute();
        }

        private void Execute()
        {
            List.SetGroup(List.Group == Group ? _default : Group);
        }
    }
}