using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ConversationNode : MasterNode
{
	[SerializeField] protected List<string> textList;

	[NonSerialized] protected VisualElement selectedTextField;

	public ConversationNode()
	{
		// ���͗p�̃|�[�g���쐬
		var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
		inputPort.portName = "Input";
		inputContainer.Add(inputPort); // ���͗p�|�[�g��inputContainer�ɒǉ�����
	}
}
