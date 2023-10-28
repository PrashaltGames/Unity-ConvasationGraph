using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
	public class RelayNode : MasterNode
	{
		public RelayNode()
		{
			title = "Relay";

			// ���͗p�̃|�[�g���쐬
			var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
			inputPort.portName = "Input";
			inputContainer.Add(inputPort);

			//�o�̓|�[�g
			var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
			outputPort.portName = "Option1";
			outputContainer.Add(outputPort);
		}
	}
}

