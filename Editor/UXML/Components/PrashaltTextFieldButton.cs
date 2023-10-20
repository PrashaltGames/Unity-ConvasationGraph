using Prashalt.Unity.ConversationGraph.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

namespace Prashalt.Unity.ConversationGraph.Components
{
    public class PrashaltTextFieldButton : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<PrashaltTextFieldButton> { }

        public PrashaltTextFieldButton()
        {
            var button = new Button
            {
                style =
                {
                    backgroundColor = Color.gray,
                    justifyContent = Justify.FlexStart,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            Add(button);

            var label = new Label
            {
                name = "label",
                text = "MainText",
            };
            label.style.marginLeft = 4;
            button.Add(label);

            var textField = new TextField
            {
                name = "mainTextField"
            };
            textField.multiline = true;
            textField.style.maxWidth = 200;
            textField.style.whiteSpace = WhiteSpace.Normal;

            //TextField�ɂ͂܂�RichText���K������Ă��Ȃ������B
            //�t�H�[�������������������ɑΉ��������Ȃ̂ŁA���}���u�͂����ɕۗ��B

            //textField.RegisterValueChangedCallback(evt =>
            //{
            //    var coloredText = ConversationGraphEditorUtility.OnChangeTextField(evt.newValue);
            //    textField.SetValueWithoutNotify(coloredText);
            //});
            button.Add(textField);
        }
	}
}
