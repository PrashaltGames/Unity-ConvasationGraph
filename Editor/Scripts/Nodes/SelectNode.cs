using Prashalt.Unity.ConversationGraph.Components;
using Prashalt.Unity.ConversationGraph.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
    [Serializable]
    public class SelectNode : MasterNode
    {
        [NonSerialized] private List<TextField> selectOptionTextList = new();
        [NonSerialized] private Button addOptionButton;
        [NonSerialized] private TemplateContainer defaultContainer;

        private const string elementPath = ConversationGraphEditorUtility.packageFilePath + "Editor/UXML/SelectNode.uxml";
        public SelectNode() : base()
        {
            title = "Select";

            // ���͗p�̃|�[�g���쐬
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float)); // ��O������Port.Capacity.Multiple�ɂ���ƕ����̃|�[�g�ւ̐ڑ����\�ɂȂ�
            inputPort.portName = "Input";
            inputContainer.Add(inputPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����

            //�o�̓|�[�g
            var outputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Option1";
            outputContainer.Add(outputPort);

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(elementPath);
            defaultContainer = visualTree.Instantiate();
            mainContainer.Add(defaultContainer);

            var textField = new PrashaltTextFiled();
            textField.Q<Label>().text = "Option1 Text";
            defaultContainer.Add(textField);

            selectOptionTextList.Add(textField.Q<TextField>());

            addOptionButton = mainContainer.Q<Button>("addOptionButton");
            addOptionButton.clicked += OnAddOptionButton;

            ConversationGraphEditorUtility.MoveUp(defaultContainer, textField);

            RefreshExpandedState();
        }
        public void OnAddOptionButton()
        {
            //�o�̓|�[�g��ǉ�
            var outputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = $"Option{outputContainer.childCount + 1}";
            outputContainer.Add(outputPort);

            var textField = new PrashaltTextFiled();
            textField.Q<Label>().text = $"Option{outputContainer.childCount} Text";

            defaultContainer.Add(textField);
            ConversationGraphEditorUtility.MoveDown(defaultContainer, addOptionButton);

            selectOptionTextList.Add(textField.Q<TextField>());
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
                selectOptionTextList[i].SetValueWithoutNotify(text);
                i++;
            }
        }
        public override string ToJson()
        {
            textList = new();
            foreach(var textField in selectOptionTextList)
            {
                textList.Add(textField.text);
            }
            return JsonUtility.ToJson(this);
        }
    }
}
