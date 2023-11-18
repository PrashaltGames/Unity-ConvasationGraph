using Prashalt.Unity.ConversationGraph;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;

public class NodeProcess
{
	public IObserver<ConversationGraphAsset> StartConversationObserver
	{
		get { return _startConversation; }
	}
	public IObservable<AnimationData> GenerateLetterAnimation
	{
		get { return _generateLetterAnimation; }
	}
	public IObservable<Unit> OnConversationFinishedEvent
	{
		get { return _onConversationFinishedEvent; }
	}
	public IObservable<ConversationInfoWithAnimationData> OnConversationNodeEvent
	{
		get { return _onConversationNodeEvent; }
	}
	public IObservable<ConversationData> OnShowOptionsEvent
	{
		get { return _onShowOptionsEvent; }
	}

	public bool IsBusy { get; private set; }
	private ConversationGraphAsset asset;
	private NodeData previousNodeData;
	internal int optionId;

	private Subject<ConversationGraphAsset> _startConversation = new();
	private Subject<AnimationData> _generateLetterAnimation = new();
	private Subject<Unit> _onConversationFinishedEvent = new();
	private Subject<Unit> _onAnimationFinished = new();
	private Subject<ConversationInfoWithAnimationData> _onConversationNodeEvent = new();
	private Subject<ConversationData> _onShowOptionsEvent = new();

	public NodeProcess()
	{
		_startConversation.Subscribe(data => StartConversation(data));
	}
	public void StartConversation(ConversationGraphAsset asset)
	{
		IsBusy = true;

		//������
		OnStartNode(asset);
		previousNodeData = asset.StartNode;
		this.asset = asset;

		//�X�^�[�g
		ProccesNodeList();
	}
	internal void OnNextNode()
	{
		ProccesNodeList();
	}
	private void ProccesNodeList()
	{
		var nodeDataList = asset.GetNextNode(previousNodeData);
		if (nodeDataList.Count <= 0)
		{
			Debug.LogError("���̃m�[�h���擾�ł��܂���ł����B");
		}
		(var node, var nodeType) = GetNodeFromLogicNode(nodeDataList);

		switch (nodeType)
		{
			case NodeType.Conversation:
				previousNodeData = node;
				OnConversationNode(node);
				break;
			case NodeType.Logic:
				OnLogicNode(node, asset);
				break;
			case NodeType.SubGraph:
				OnSubGraphNode(node);
				break;
			case NodeType.Error:
				Debug.LogError("Error");
				break;
			case NodeType.End:
				_onConversationFinishedEvent.OnNext(Unit.Default);
				break;
		}
	}
	private (NodeData, NodeType) GetNodeFromLogicNode(in List<NodeData> nodeDataList)
	{
		int nodeCount = 0;
		NodeData node = null;
		NodeType nodeType = NodeType.Error;
		foreach (var nodeData in nodeDataList)
		{
			//Logic�n�iSelect�͗�O�I�Ɋ܂ށF�C���K�v�����j�̎��͂��̔ԍ��݂̂��Đ�����
			if (optionId != nodeCount)
			{
				nodeCount++;
				continue;
			}

			nodeType = GetNodeType(nodeData);

			nodeCount++;
			node = nodeData;
		}
		optionId = 0;
		return (node, nodeType);
	}
	private NodeType GetNodeType(in NodeData nodeData)
	{
		var typeName = nodeData.typeName.Split(".")[4];
		//�m�[�h�𕪐�
		switch (typeName)
		{
			case "Conversation":
				return NodeType.Conversation;
			case "Logic":
				return NodeType.Logic;
			case "SubGraphNode":
				return NodeType.SubGraph;
			//EndNode�̂Ƃ�
			case "EndNode":
				return NodeType.End;
		}
		return NodeType.Error;
	}
	private void OnStartNode(in ConversationGraphAsset asset)
	{
		var previousNodeData = asset.StartNode;
		var startNodeData = JsonUtility.FromJson<ConversationData>(previousNodeData.json);

		//StartNode����A�j���[�V������Presenter�w�Ŏ擾�B
		var animationNodeData = asset.FindNode(startNodeData.animationGuid);
		if(animationNodeData is null) return;
		var animationData = JsonUtility.FromJson<AnimationData>(animationNodeData.json);
		_generateLetterAnimation.OnNext(animationData);
	}
	private void OnConversationNode(in NodeData nodeData)
	{
		var conversationData = JsonUtility.FromJson<ConversationData>(nodeData.json);
		if (nodeData.typeName.Split(".")[5] == "SelectNode")
		{
			ConversationLogic.ConversationInput.State.Value = ConversationState.WaitSelect;
			Debug.Log(ConversationLogic.ConversationInput.State.Value);
			_onShowOptionsEvent.OnNext(conversationData);
		}
		else
		{
			ConversationLogic.ConversationInput.State.Value = ConversationState.LetterAnimation;
			//�X�s�[�J�[�l�[����ݒ�
			var speakerName = ReflectProperty(conversationData.speakerName);

			//�A�j���[�V��������ݒ�
			var animationNodeData = asset.FindNode(conversationData.animationGuid);
			AnimationData animationData;
			if (animationNodeData is not null)
			{
				animationData = JsonUtility.FromJson<AnimationData>(animationNodeData.json);
			}
			else
			{
				animationData = new("None");
			}

			foreach (var text in conversationData.textList)
			{
				var reflectedPropertyText = ReflectProperty(text);
				ConversationInfoWithAnimationData conversationInfoWithAnimationName = new(speakerName, reflectedPropertyText, animationData);
				_onConversationNodeEvent.OnNext(conversationInfoWithAnimationName);
			}
		}
	}
	private void OnLogicNode(NodeData nodeData, ConversationGraphAsset asset)
	{
		//�f�[�^��ǂݎ��B
		var logicData = JsonUtility.FromJson<LogicData>(nodeData.json);

		var boolGuid = logicData.inputNodeGuids[0];
		var boolNodeData = asset.FindNode(boolGuid);
		var boolNode = JsonUtility.FromJson<PropertyData>(boolNodeData.json);

		//boolNode�̏����擾
		var boolInfo = ConversationGraphUtility.ConversationProperties[boolNode.memberName];
		bool value = false;
		if (boolInfo is PropertyInfo property)
		{
			value = (bool)property.GetValue(null);
		}
		else if (boolInfo is FieldInfo field)
		{
			value = (bool)field.GetValue(null);
		}

		if (value)
		{
			optionId = 0;
		}
		else
		{
			optionId = 1;
		}
	}
	private void OnSubGraphNode(NodeData nodeData)
	{
		//�f�[�^��ǂݎ��B
		var subGraphData = JsonUtility.FromJson<SubGraphData>(nodeData.json);

		StartConversation(subGraphData.subGraph);
	}

	protected string ReflectProperty(string text)
	{
		if (text is null || text == "") return "";
		var Matches = new Regex(@"\{(.+?)\}").Matches(text);

		foreach (Match propertyNameMatch in Matches)
		{
			//���K�\��������Ȃ��̂ŁA�S������
			var propertyName = propertyNameMatch.Value.Replace("{", "");
			propertyName = propertyName.Replace("}", "");

			var hasProperty = ConversationGraphUtility.ConversationProperties.TryGetValue(propertyName, out var member);
			if (!hasProperty)
			{
				Debug.LogError("Property is missing maybe");
				continue;
			}
			string value = "";
			if (member is PropertyInfo property)
			{
				value = property.GetValue(null).ToString();
			}
			else if (member is FieldInfo field)
			{
				value = field.GetValue(null).ToString();
			}
			text = text.Replace($"{{{propertyName}}}", value);
		}
		return text;
	}
}
internal enum NodeType
{
	Conversation,
	Logic,
	SubGraph,
	End,

	Error
}
public struct ConversationInfo
{
	public string SpeakerName { get; internal set; }
	public string Text { get; internal set; }
	public ConversationInfo(string speackerName, string text)
	{
		SpeakerName = speackerName;
		Text = text;
	}
}
public struct ConversationInfoWithAnimationData
{
	public ConversationInfo ConversationInfo { get; private set; }
	public AnimationData AnimationData { get; private set; }

	public ConversationInfoWithAnimationData(string speackerName, string text, AnimationData animationData)
	{
		ConversationInfo = new(speackerName, text);
		AnimationData = animationData;
	}
}
