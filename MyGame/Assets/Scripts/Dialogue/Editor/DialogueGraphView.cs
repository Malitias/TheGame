using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150,200);

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("GridStyle"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger() );
        this.AddManipulator(new SelectionDragger() );
        this.AddManipulator(new RectangleSelector() );

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());

    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) => {
            if (startPort != port && startPort.node != port.node) {
                compatiblePorts.Add(port);
            };
        });

        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity=Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal,portDirection,capacity, typeof(float) );
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "start",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName) {
        AddElement( CreateDialogueNode(nodeName));
    }

    public DialogueNode CreateDialogueNode(string nodeName) {

        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(()=> {
            AddChoicePort(dialogueNode);
        });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode ,string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        

        var choicePortname = string.IsNullOrEmpty(overriddenPortName) 
            ? $"Choice {outputPortCount +1}" : overriddenPortName;


        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortname
        };

        var oldLabel = generatedPort.contentContainer.Q<Label>("type"); 
        textField.RegisterValueChangedCallback(evt => {
            generatedPort.portName = evt.newValue;
            oldLabel.text = " ";
        });


        var deletButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };

        generatedPort.contentContainer.Add(textField);
        generatedPort.contentContainer.Add(deletButton);

        generatedPort.portName = choicePortname;
        
        oldLabel.text = " ";



        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node==generatedPort.node);
        if (!targetEdge.Any())
        {
            return;
        }
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
