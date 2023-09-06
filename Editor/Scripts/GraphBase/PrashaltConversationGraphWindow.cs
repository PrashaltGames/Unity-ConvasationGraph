using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;

namespace Prashalt.Unity.ConversationGraph.Editor
{
    public class PrashaltConversationWindow : EditorWindow
    {
        public ConversationGraphAsset ConvasationGraphAsset { set; get; }
        public PrashaltConversationGraph convasationGraphView;

        public static List<PrashaltConversationWindow> activeWindowList = new();

        private bool isAssetSet = false;

        public void Open(ConversationGraphAsset convasationGraphAsset)
        {
            ConvasationGraphAsset = convasationGraphAsset;

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
                        if (window.ConvasationGraphAsset.GetInstanceID() == conversationGraphAsset.GetInstanceID())
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
            if (ConvasationGraphAsset is null) return;

            ConvasationGraphAsset.ClearNodes();

            foreach(var node in convasationGraphView.nodes)
            {
                if (node is MasterNode)
                {
                    ConvasationGraphAsset.SaveNode(ConversationGraphEditorUtility.NodeToData(node as MasterNode));
                }
                else if(node is GraphInspectorNode graphInspector)
                {
                    ConvasationGraphAsset.settings.isNeedClick = graphInspector.isNeedClick;
                    ConvasationGraphAsset.settings.shouldTextAnimation = graphInspector.shouldTextAnimation;
                    ConvasationGraphAsset.settings.switchingSpeed = graphInspector.switchingSpeed;
                    ConvasationGraphAsset.settings.animationSpeed = graphInspector.animationSpeed;
                }
            }

            ConvasationGraphAsset.ClearEdges();
            foreach(var edge in convasationGraphView.edges)
            {
                var edgeData = ConversationGraphEditorUtility.EdgeToData(edge);
                if (edgeData is null) continue;

                ConvasationGraphAsset.SaveEdge(edgeData);
            }

            EditorUtility.SetDirty(ConvasationGraphAsset);
            AssetDatabase.SaveAssets();
        }
        private static void CreateNewWindow(ConversationGraphAsset conversationGraphAsset)
        {
            var newWindow = CreateWindow<PrashaltConversationWindow>(typeof(SceneView));

            newWindow.Open(conversationGraphAsset);
            newWindow.titleContent.text = conversationGraphAsset.name;
            newWindow.Focus();

            activeWindowList.Add(newWindow);
        }
    }

}