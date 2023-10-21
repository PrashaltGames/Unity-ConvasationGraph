using Cysharp.Threading.Tasks;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
	[Serializable]
	public class SubGraphNode : MasterNode
	{
		[SerializeField] public ConversationGraphAsset subGraph;

		//private int clickCount;
		public SubGraphNode()
		{
			var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
			inputPort.portName = "Input";
			inputContainer.Add(inputPort);

			// �o�͗p�̃|�[�g�����
			var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
			outputPort.portName = "Output";
			outputContainer.Add(outputPort);

			//RegisterCallback<ClickEvent>(DoubleClick);
		}
		public void SetSubGraphAsset(ConversationGraphAsset subGraph)
		{
			this.subGraph = subGraph;
			title = $"{subGraph.name} (Sub Graph)";
		}
		public override string ToJson()
		{
			return JsonUtility.ToJson(this);
		}


		//RegisterCallback���Ȃ��������Ȃ��̂ŕۗ�
		//private async void DoubleClick(ClickEvent e)
		//{
		//	clickCount = e.clickCount;

		//	await UniTask.Delay(500);
		//	ExecuteDoubleClick();
		//}
		//private void ExecuteDoubleClick()
		//{
		//	if(clickCount > 1)
		//	{
		//		Debug.Log("�_�u���N���b�N�I");
		//	}
		//	clickCount = 0;
		//}
	}
}
