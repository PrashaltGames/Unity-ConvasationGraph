using Cysharp.Threading.Tasks;
using Prashalt.Unity.ConversationGraph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Editor
{
    public class PrashaltConversationGraph : GraphView
    {
        public PrashaltConversationWindow _window;
#region Func_UnityEditor
        public PrashaltConversationGraph(PrashaltConversationWindow window)
        {
            //ScritableObject����ǉ�����B
            _window = window;
            _window.convasationGraphView = this;

            // �e�̃T�C�Y�ɍ��킹��GraphView�̃T�C�Y��ݒ�
            this.StretchToParentSize();

            // MMB�X�N���[���ŃY�[���C���A�E�g���ł���悤��
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            // MMB�h���b�O�ŕ`��͈͂𓮂�����悤��
            this.AddManipulator(new ContentDragger());
            // LMB�h���b�O�őI�������v�f�𓮂�����悤��
            this.AddManipulator(new SelectionDragger());
            // LMB�h���b�O�Ŕ͈͑I�����ł���悤��
            this.AddManipulator(new RectangleSelector());

            // �E�N���b�N���j���[��ǉ�
            var menuWindowProvider = ScriptableObject.CreateInstance<PrashaltSearchMenuWindowProvider>();
            menuWindowProvider.Initialize(this, _window);
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            };

            if (_window.ConvasationGraphAsset is null || _window.ConvasationGraphAsset.Nodes.Count <= 0)
            {
                var startNode = new StartNode();
                AddElement(startNode);
                var endNode = new EndNode();
                AddElement(endNode);

                startNode.Initialize(null, new Rect(100f, 150f, default, default), "");
                endNode.Initialize(null, new Rect(800f, 150f, default, default), "");
            }
            else
            {
                ShowNodesFromAsset(_window.ConvasationGraphAsset);
                ShowEdgeFromAsset(_window.ConvasationGraphAsset);
            }
        }
        // GetCompatiblePorts���I�[�o�[���C�h����
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            compatiblePorts.AddRange(ports.ToList().Where(port =>
            {
                // �����m�[�h�ɂ͌q���Ȃ�
                if (startPort.node == port.node)
                    return false;

                // Input���m�AOutput���m�͌q���Ȃ�
                if (port.direction == startPort.direction)
                    return false;

                // �|�[�g�̌^����v���Ă��Ȃ��ꍇ�͌q���Ȃ�
                if (port.portType != startPort.portType)
                    return false;

                return true;
            }));

            return compatiblePorts;
        }
        #endregion
        #region Func_Original
        public void ShowNodesFromAsset(ConversationGraphAsset asset)
        {
            foreach(var nodeData in asset.Nodes)
            {
                var t = Type.GetType(nodeData.typeName);
                if (t is null) continue;

                var instance = Activator.CreateInstance(t) as MasterNode;
                if (instance is null) continue;

                AddElement(instance);
                instance.Initialize(nodeData.guid, nodeData.rect, nodeData.json);
            }
        }
        public async void ShowEdgeFromAsset(ConversationGraphAsset asset)
        {
            await UniTask.Delay(10);
            MasterNode previousBaseNode = null;
            int outputIndex = 0;
            foreach (var edgeData in asset.Edges)
            {
                var baseNode = nodes.Select(x => x as MasterNode).FirstOrDefault(x => x.guid == edgeData.baseNodeGuid);
                var targetNode = nodes.Select(x => x as MasterNode).FirstOrDefault(x => x.guid == edgeData.targetNodeGuid);
                if (baseNode is null || targetNode is null) return;

                //�O�Ɠ����Ȃ�Q�ڂ𖄂߂�
                if(previousBaseNode is not null && baseNode.guid == previousBaseNode.guid)
                {
                    outputIndex++;
                }
                else
                {
                    outputIndex = 0;
                }

                var input = targetNode.inputContainer.Children().FirstOrDefault(x => x is Port) as Port;
                var outputs = baseNode.outputContainer.Children().Where(x => x is Port);

                int i = 0;
                foreach(Port output in outputs)
                {
                    if (input == null || output == null) return;
                    if (outputIndex != i)
                    {
                        i++;
                        continue;
                    }

                    var edge = new Edge() { input = input, output = output };
                    edge.input.Connect(edge);
                    edge.output.Connect(edge);
                    Add(edge);
                    i++;
                }

                previousBaseNode = baseNode;
            }
            
        }
        #endregion
    }
}
