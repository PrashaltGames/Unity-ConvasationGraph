using Prashalt.Unity.ConversationGraph.Components;
using Prashalt.Unity.ConversationGraph.Editor;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Nodes.Conversation
{
    [Serializable]
    public class SelectNode : ConversationNode
    {
        private const string elementPath = ConversationGraphEditorUtility.packageFilePath + "Editor/UXML/SelectNode.uxml";
        public SelectNode() : base(elementPath, "Option Text")
        {
            title = "Select";

            //�o�̓|�[�g
            AddOutputPort(typeof(float), "Option 1");

            var addOptionButton = mainContainer.Q<Button>("addButton");
            addOptionButton.clicked += OnAddOptionButton;

            RefreshExpandedState();
        }
		public void OnAddOptionButton()
		{
			//�o�̓|�[�g��ǉ�
            AddOutputPort(typeof(float), $"Option {outputContainer.childCount + 1}");
            
			//���͗���ǉ�
			var textField = new PrashaltTextFieldButton();
			textField.Q<Label>().text = $"Option Text {outputContainer.childCount}";
			textField.Q<Button>().clicked += () => SelectTextButton(textField);

			defaultContainer.Add(textField);
			ConversationGraphEditorUtility.MoveDown(defaultContainer, buttonContainer);

			textFieldList.Add(textField.Q<TextField>());
		}

		public override void Initialize(string guid, Rect rect, string json)
        {
            base.Initialize(guid, rect, json);
            var jsonObj = JsonUtility.FromJson<SelectNode>(json);
            int i = 0;
            if (jsonObj is null) return;
            foreach(var text in jsonObj.textList)
            {
                if (i > 0)
                {
                    OnAddOptionButton();
                }
                textFieldList[i].SetValueWithoutNotify(text);
                i++;
            }
        }
        public override string ToJson()
        {
            textList = new();
            foreach(var textField in textFieldList)
            {
                textList.Add(textField.text);
            }
            return JsonUtility.ToJson(this);
        }
    }
}
