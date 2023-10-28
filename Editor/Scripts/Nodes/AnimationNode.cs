using Prashalt.Unity.ConversationGraph.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
	[Serializable]
	public class AnimationNode<T> : MasterNode where T : ConversationAnimationGenerator
	{
		[SerializeField] private List<int> intProperties = new();
		[SerializeField] private List<float> floatProperties = new();
		[SerializeField] private string animationName;
		public AnimationNode()
		{
			title = $"{typeof(T).Name} (Animation)";

			//�o�̓|�[�g
			var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(T));
			outputPort.portName = "Output";
			outputPort.portColor = Color.red;
			outputContainer.Add(outputPort);

			animationName = typeof(T).Name;		
		}
		public override void Initialize(string guid, Rect rect, string json)
		{
			base.Initialize(guid, rect, json);
			var obj = JsonUtility.FromJson<AnimationNode<T>>(json);

			if(obj is not null)
			{
				intProperties = obj.intProperties;
				floatProperties = obj.floatProperties;
			}


			//�v���p�e�B���w��ł���悤��
			var fields = typeof(T).GetFields();
			AddPropertyTextField<int>(fields);
			AddPropertyTextField<float>(fields);
		}
		public void AddPropertyTextField<T1>(FieldInfo[] infoArray)
		{
			var fields = infoArray.Where(x => x.FieldType == typeof(T1));

			var intIndex = 0;
			var floatIndex = 0;
			foreach(var info in fields)
			{
				if (typeof(T1) == typeof(int))
				{
					var integerField = new IntegerField
					{
						label = info.Name
					};
					if (intProperties.Count - 1 < intIndex)
					{
						intProperties.Add(0);
					}
					else
					{
						integerField.SetValueWithoutNotify(intProperties[intIndex]);
					}

					//�C�x���g��o�^
					int i = intIndex;
					integerField.RegisterValueChangedCallback(e => {
						intProperties[i] = e.newValue;
					});
					mainContainer.Add(integerField);

					intIndex++;
				}
				else if (typeof(T1) == typeof(float))
				{
					var floatField = new FloatField
					{
						label = info.Name
					};
					//�v���p�e�B�̐��ɑ΂��Ĕz�񐔂����Ȃ�������ǉ��B
					if(floatProperties.Count - 1 < floatIndex)
					{
						floatProperties.Add(0);
					}
					else
					{
						floatField.SetValueWithoutNotify(floatProperties[floatIndex]);
					}

					//�C�x���g��o�^
					int i = floatIndex;
					floatField.RegisterValueChangedCallback(e => {
						floatProperties[i] = e.newValue;
					});
					mainContainer.Add(floatField);

					floatIndex++;
				}
				else
				{
					Debug.LogError("�p�b�P�[�W�J���҂���~�X���Ă܂���");
				}
			}
		}
		public override string ToJson()
		{
			return JsonUtility.ToJson(this);
		}
	}
}
