using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prashalt.Unity.ConversationGraph.Nodes
{
    public class StartNode : MasterNode
    {
        public StartNode()
        {
            title = "Start";

            // �o�͗p�̃|�[�g�����
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Value";
            outputContainer.Add(outputPort); // �o�͗p�|�[�g��outputContainer�ɒǉ�����

            capabilities &= ~Capabilities.Deletable;
        }
    }
}
