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
			AddInputPort(typeof(float));

			//�o�̓|�[�g
			AddInputPort(typeof(float));
		}
	}
}

