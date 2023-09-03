using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
    [Serializable]
    public class EndNode : MasterNode
    {
        public EndNode()
        {
            title = "End";

            // ���͗p�̃|�[�g���쐬
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float)); // ��O������Port.Capacity.Multiple�ɂ���ƕ����̃|�[�g�ւ̐ڑ����\�ɂȂ�
            inputPort.portName = "End";
            inputContainer.Add(inputPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����
        }

        public override void Initialize(string guid, Rect rect, string json)
        {
            base.Initialize(guid, rect, json);
        }
    }
}