using Prashalt.Unity.ConversationGraph.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ConversationGraphUtility
{
	public static readonly Dictionary<string, MemberInfo> ConversationProperties;

	public const string ERRORMESSAGE = "ConversationGraph : ConversationProperty use only static";
	static ConversationGraphUtility()
	{
		ConversationProperties = GetAllConversationProperties();
	}
	public static Dictionary<string, MemberInfo> GetAllConversationProperties()
	{
		var dic = new Dictionary<string, MemberInfo>();

		var targetAssembyName = typeof(ConversationPropertyAttribute).Assembly.GetName().FullName;
		
		//�A�Z���u���ꗗ���擾
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			//�A�Z���u���̈ˑ��֌W���擾
			var referencedAssemblies = assembly.GetReferencedAssemblies();
			foreach(var referencedAssemblyName in referencedAssemblies)
			{
				//ConversationProperty�Ɉˑ����Ă��Ȃ��������΂��B
				if(referencedAssemblyName.FullName != targetAssembyName)
				{
					continue;
				}

				//���Ă�����Property��T���Ĉꗗ�ɒǉ�
				var classList = GetHasConversationPropertyClasses(assembly);
				foreach (var typeInfo in classList)
				{
					var properties = GetConversationProperties(typeInfo);

					foreach (var info in properties)
					{
#if UNITY_EDITOR
						if(info is FieldInfo field)
						{
							if(field.IsStatic)
							{
								dic.Add(info.Name, info);
							}
							else
							{
								Debug.LogError($"{ERRORMESSAGE} : {field.Name}");
							}
						}
						else if (info is PropertyInfo property)
						{
							if (property.GetMethod.IsStatic)
							{
								dic.Add(info.Name, info);
							}
							else
							{
								Debug.LogError($"{ERRORMESSAGE} : {property.Name}");
							}
						}
#else
						dic.Add(info.Name, info);
#endif
					}
				}
			}
		}
		return dic;
	}
	private static IEnumerable<TypeInfo> GetHasConversationPropertyClasses(Assembly assembly)
	{
		foreach (Type type in assembly.GetTypes())
		{
			if (type.GetCustomAttributes(typeof(HasConversationPropertyAttribute), true).Length > 0)
			{
				yield return type.GetTypeInfo();
			}
		}
	}
	private static IEnumerable<MemberInfo> GetConversationProperties(TypeInfo type)
	{
		foreach (MemberInfo info in type.GetMembers())
		{
			if (info.IsDefined(typeof(ConversationPropertyAttribute), true))
			{
				yield return info;
			}
		}
	}
}
