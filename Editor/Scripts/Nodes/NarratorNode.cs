using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConvasationGraph.Nodes
{
    [Serializable]
    public class NarratorNode : MasterNode
    {
        [SerializeField] protected string text;
        [NonSerialized] protected TextField _textField;

        private const string elementPath = test + "Editor/UXML/NarratorNode.uxml";
        private const string test = "Assets/PrashaltConvasationGraph/";
        public NarratorNode()
        {
            title = "Narrator";

            // ���͗p�̃|�[�g���쐬
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float)); // ��O������Port.Capacity.Multiple�ɂ���ƕ����̃|�[�g�ւ̐ڑ����\�ɂȂ�
            inputPort.portName = "Input";
            inputContainer.Add(inputPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����

            //�o�̓|�[�g
            var outputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(elementPath);
            var template = visualTree.Instantiate();
            mainContainer.Add(template);

            _textField = mainContainer.Q<TextField>("mainTextField");
        }

        public override void Initialize(string guid, Rect rect, string json)
        {
            base.Initialize(guid, rect, json);
            var jsonObj = JsonUtility.FromJson<NarratorNode>(json);
            _textField.SetValueWithoutNotify(jsonObj?.text);
        }
        public override string ToJson()
        {
            text = _textField.text;
            return JsonUtility.ToJson(this);
        }
    }
}