using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Prashalt.Unity.ConversationGraph.Nodes.Logic
{
	[Serializable]
	public class BranchNode : LogicNode
	{
		public BranchNode()
		{
			title = "Branch";

			// ���͗p�̃|�[�g���쐬
			var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float)); // ��O������Port.Capacity.Multiple�ɂ���ƕ����̃|�[�g�ւ̐ڑ����\�ɂȂ�
			inputPort.portName = "Input";
			inputContainer.Add(inputPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����

			var boolPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool)); // ��O������Port.Capacity.Multiple�ɂ���ƕ����̃|�[�g�ւ̐ڑ����\�ɂȂ�
			boolPort.portName = "Bool";
			boolPort.portColor = Color.magenta;
			inputContainer.Add(boolPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����

			//�o�̓|�[�g
			var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
			outputPort.portName = "True";
			outputContainer.Add(outputPort);

			var outputPort1 = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
			outputPort1.portName = "False";
			outputContainer.Add(outputPort1);
		}
		public override string ToJson()
		{
			int i = 0;
			foreach (Port input in inputContainer.Children())
			{
				foreach(var edge in input.connections)
				{
					if(edge.output.node is MasterNode masterNode)
					{
						switch(i)
						{
							case 0:
								break;
							case 1:
								inputNodeGuids.Add($"B:{masterNode.guid}");
								break;
						}
					}
				}
				i++;
			}
			i = 0;
			foreach(Port output in outputContainer.Children())
			{
				foreach (var edge in output.connections)
				{
					if (edge.output.node is MasterNode masterNode)
					{
						switch (i)
						{
							case 0:
								outputNodeGuids.Add($"T:{masterNode.guid}");
								break;
							case 1:
								outputNodeGuids.Add($"F:{masterNode.guid}");
								break;
						}
					}
				}
				i++;
			}
			return JsonUtility.ToJson(this);
		}
	}
}