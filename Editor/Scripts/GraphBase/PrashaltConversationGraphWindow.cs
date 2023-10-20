using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Editor
{
    public class PrashaltConversationWindow : EditorWindow
    {
        public ConversationGraphAsset ConversationGraphAsset { set; get; }
        public PrashaltConversationGraph conversationGraphView;

        public static List<PrashaltConversationWindow> activeWindowList = new();

        private bool isAssetSet = false;

        private const string iconPath = ConversationGraphEditorUtility.packageFilePath + "Editor/Icon/ConversationGraphTab.png";

		public void OnGUI()
		{
			conversationGraphView.DropSubGraph();
		}
		public void Open(ConversationGraphAsset convasationGraphAsset)
        {
            ConversationGraphAsset = convasationGraphAsset;

            isAssetSet = true;

            Show();
        }
        private async void OnEnable()
        {
            rootVisualElement.Clear();

            await UniTask.WaitUntil(() => isAssetSet);
            var graphView = new PrashaltConversationGraph(this);
            rootVisualElement.Add(graphView);

            var toolvar = new Toolbar();
            var saveButton = new ToolbarButton(OnSave) { text = "Save", name = "save-button" };
            toolvar.Add(saveButton);
            rootVisualElement.Add(toolvar);
        }
        private void OnDisable()
        {
            activeWindowList.Remove(this);
        }
        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceId, int _)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is ConversationGraphAsset)
            {
                var conversationGraphAsset = EditorUtility.InstanceIDToObject(instanceId) as ConversationGraphAsset;

                if (HasOpenInstances<PrashaltConversationWindow>())
                {
                    foreach (var window in activeWindowList)
                    {
                        if (window.ConversationGraphAsset.GetInstanceID() == conversationGraphAsset.GetInstanceID())
                        {
                            window.Focus();
                            return false;
                        }
                    }
                    CreateNewWindow(conversationGraphAsset);
                    return false;
                }
                else
                {
                    // �V�Kwindow�쐬
                    CreateNewWindow(conversationGraphAsset);
                    return true;
                }
            }

            return false;
        }
        public void OnSave()
        {
            if (ConversationGraphAsset is null) return;

            ConversationGraphAsset.ClearNodes();
            ConversationGraphAsset.ClearEdges();

            foreach (var node in conversationGraphView.nodes)
            {
                if (node is MasterNode masterNode)
                {
                    var nodeData = ConversationGraphEditorUtility.NodeToData(masterNode);
                    ConversationGraphAsset.SaveNode(nodeData);
                }
                else if(node is GraphInspectorNode graphInspector)
                {
                    ConversationGraphAsset.settings.isNeedClick = graphInspector.isNeedClick;
                    ConversationGraphAsset.settings.shouldTextAnimation = graphInspector.shouldTextAnimation;
                    ConversationGraphAsset.settings.switchingSpeed = graphInspector.switchingSpeed;
                    ConversationGraphAsset.settings.animationSpeed = graphInspector.animationSpeed;
                }
            }

            ConversationGraphAsset.ClearEdges();
            foreach (var edge in conversationGraphView.edges)
            {
                var edgeData = ConversationGraphEditorUtility.EdgeToData(edge);
                if (edgeData is null) continue;

                var inputOptionId = 0;
                foreach (var parentChild in edge.input.parent.Children())
                {
                    if (parentChild == edge.input)
                    {
                        break;
                    }
                    inputOptionId++;
                }
                edgeData.targetNodeGuid += $":{inputOptionId}";

                var outputOptionId = 0;
                foreach (var parentChild in edge.output.parent.Children())
                {
                    if (parentChild == edge.output)
                    {
                        break;
                    }
                    outputOptionId++;
                }
                edgeData.baseNodeGuid += $":{outputOptionId}";

                ConversationGraphAsset.SaveEdge(edgeData);
            }

            EditorUtility.SetDirty(ConversationGraphAsset);
            AssetDatabase.SaveAssets();
        }
        private static void CreateNewWindow(ConversationGraphAsset conversationGraphAsset)
        {
            var newWindow = CreateWindow<PrashaltConversationWindow>(typeof(SceneView));
            var icon = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);

            newWindow.Open(conversationGraphAsset);
            newWindow.titleContent = new(conversationGraphAsset.name, icon);
            newWindow.Focus();

            activeWindowList.Add(newWindow);
        }
    }

}