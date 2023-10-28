using System.Collections.Generic;
using System.Linq;

namespace Monitoring.Display
{
    public class Node
    {
        public Node Parent;
        public string PathName;
        int Depth = 0;
        internal List<Node> Nodes = new List<Node>();
        public List<TrackedValue> Values = new List<TrackedValue>();
        public List<TrackedMethod> Methods = new List<TrackedMethod>();

        public Node(Node Parent, string pathName, int depth)
        {
            this.Parent = Parent;
            Depth = depth;
            PathName = pathName;
        }

        public void AddValue(string[] valuePath, TrackedObject value)
        {
            if(Depth == valuePath.Length - 1)
            {
                if(value is TrackedValue v)
                    Values.Add(v);
                else if (value is TrackedMethod m)
                    Methods.Add(m);
            } 
            else
            {
                var pathSegment = valuePath[Depth + 1];
                var node = Nodes.Find(x => x.PathName == pathSegment);
                if (node == null)
                {
                    node = new Node(this, pathSegment, Depth + 1);
                    Nodes.Add(node);
                    node.AddValue(valuePath, value);
                }
                else node.AddValue(valuePath, value);
            }
        }

        public void AddValue(TrackedValue value)
        {
            Values.Add(value);
        }

        public void AddMethod(TrackedMethod method)
        {
            Methods.Add(method);
        }
    }

    public class MonitorTree
    {
        object MonitoredObject;
        public Node RootNode;

        public MonitorTree(object monitoredObject, TrackedValue[] values, TrackedMethod[] methods)
        {
            MonitoredObject = monitoredObject;
            RootNode = new Node(null, monitoredObject.ToString(), 0);

            foreach (TrackedValue value in values)
            {
                var cat = value.Options.Category;
                if (cat == "")
                {
                    RootNode.AddValue(value);
                }
                else if (!cat.Contains('/'))
                {
                    cat = monitoredObject.ToString() + "/" + cat;
                    var path = cat.Split('/');
                    RootNode.AddValue(path, value);
                }
                else
                {
                    var path = cat.Split('/');
                    RootNode.AddValue(path, value);
                }
            }
            
            foreach (TrackedMethod method in methods)
            {
                var cat = method.Options.Category;
                if (cat == "")
                {
                    RootNode.AddMethod(method);
                }
                else if (!cat.Contains('/'))
                {
                    cat = monitoredObject.ToString() + "/" + cat;
                    var path = cat.Split('/');
                    RootNode.AddValue(path, method);
                }
                else
                {
                    var path = cat.Split('/');
                    RootNode.AddValue(path, method);
                }
            }
        }
    }
}