using Prashalt.Unity.ConversationGraph.Animation;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
    [Serializable]
    public class StartNode : MasterNode
    {
		[SerializeField] private string animationGuid;

		[NonSerialized] private Port animationPort;
        public StartNode()
        {
            title = "Start";

            // �o�͗p�̃|�[�g�����
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Start";
            outputContainer.Add(outputPort); // �o�͗p�|�[�g��outputContainer�ɒǉ�����

			// ���͗p�̃|�[�g���쐬
			animationPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(LetterAnimation));
			animationPort.portName = "Letters Animation";
			animationPort.portColor = Color.red;
			inputContainer.Add(animationPort);

			capabilities &= ~Capabilities.Deletable;
		}
		public override string ToJson()
		{
			base.ToJson();
			if (animationPort.connected)
			{
				var edge = animationPort.connections.FirstOrDefault();

				var animationNode = edge.output.node as MasterNode;
				animationGuid = animationNode.guid;
			}
			return JsonUtility.ToJson(this);
		}
	}
}
