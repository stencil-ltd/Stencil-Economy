using Binding;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Stencil.Economy.Ui
{
    [RequireComponent(typeof(Text))]
    public class TermsText : MonoBehaviour
    {
        public string appleProvider = "your iTunes Account";
        public string androidProvider = "Google Play";

        [Bind] private Text _text;

        private void Awake()
        {
            this.Bind();
        }

        private void Start()
        {
            var platform = new PlatformValue<string>(appleProvider).WithAndroid(androidProvider);
            _text.text = _text.text.Replace("{PROVIDER}", platform);
        }
    }
}