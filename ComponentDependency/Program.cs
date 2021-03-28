using System;
using System.Collections.Generic;
using System.Text;


namespace ComponentDependency
{
    class Program
    {
        static void Main(string[] args)
        {

            DoComponentDependency();

            Do_DFS_BFS();
        }

        public static void Do_DFS_BFS()
        {
            //[[1,0],[2,0],[0,5],[5,6],[6,6],[3,1],[3,2]] start = 3
            //(01)(02)(12)(20)(23)(33) Start = 2
            
            Graph graph = new Graph();
            graph.AddEdge(1, 0);
            graph.AddEdge(2, 0);
            graph.AddEdge(0, 5);
            graph.AddEdge(5, 6);
            graph.AddEdge(6, 6);
            graph.AddEdge(3, 1);
            graph.AddEdge(3, 2);

            graph.DFS(3);
            graph.ResetAdjList();
            graph.BFS(3);
        }

        public static void DoComponentDependency()
        {
            string[] inputs = {
                "DEPEND TELNET TCPIP NETCARD",
                "DEPEND TCPIP NETCARD",
                "DEPEND DNS TCPIP NETCARD",
                "DEPEND BROWSER TCPIP HTML",
                "INSTALL NETCARD",
                "INSTALL TELNET",
                "INSTALL FOO",
                "INSTALL DNS",
                "INSTALL BROWSER",
                "LIST",
                "REMOVE NETCARD",
                "REMOVE FOO",
                "REMOVE BROWSER",
                "LIST",
                "END"
            };
            Console.WriteLine("******Inputs********");
            foreach (string s in inputs)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("******Outputs********");
            DoIt(inputs);
        }

        public static void DoIt(string[] inputs)
        {
            List<Component> components = new List<Component>();

            foreach (string input in inputs)
            {
                string[] words = input.Split(' ');
                string command = words[0];
               

                if(command == "DEPEND")
                {
                    CreateDependency(components, words);

                }else if (command == "INSTALL")
                {
                    string componentname = words[1];
                    InstallComponent(components, componentname, true);

                }else if (command == "REMOVE")
                {
                    string componentname = words[1];
                    RemoveComponent(components, componentname,true);
                }else if(command == "LIST")
                {
                    List(components);
                }else if(command == "END")
                {
                    //remove unused component
                    components  = RemoveUnusedComponent(components);
                    Console.WriteLine("END");
                    break;
                }
            }

        }

        public static void CreateDependency(List<Component> components, string[] input)
        {
            string comName = input[1];
            Component component = components.Find(c => c.Name == comName);
            if(component == null)
            {
                component = new Component(comName);
                components.Add(component);
            }
            Console.Write($"{input[0]} {input[1]}");
            for(int i=2; i < input.Length; i++)
            {
                component.Dependencies.Add(input[i]);
                Console.Write($" {input[i]}");
            }
            Console.WriteLine();
            
        }

        public static void InstallComponent(List<Component> components, string comName, bool isexplicility)
        {
            if (isexplicility)
            {
                Console.WriteLine($"INSTALL {comName}");
            }

            Component component = components.Find(c => c.Name == comName);
            if (component == null)
            {
                Console.WriteLine($"Installing {comName}");
                component = new Component(comName);
                component.IsInstalled = true;
                component.InstalledExplicitly = isexplicility;
                components.Add(component); 
            }
            else
            {
                if (!component.IsInstalled)
                {
                    Console.WriteLine($"Installing {comName}");
                    component.IsInstalled = true;
                    component.InstalledExplicitly = isexplicility;
                }
                //check for dependency and install them implicitlity
                foreach (string depe in component.Dependencies)
                {
                    InstallComponent(components, depe, false);
                }

            }
        }

        public static void RemoveComponent(List<Component> components, string comName, bool commandremove)
        {
            if (commandremove)
            {
                Console.WriteLine($"REMOVE {comName}");
            }

            bool isremove = true;
            foreach(Component c in components.FindAll(com=> com.Name != comName))
            {
                if(c.IsInstalled && c.Dependencies.Exists(d=> d == comName))
                {
                    isremove = false;
                    Console.WriteLine($"{comName} is still needed");
                    break;
                }
            }

            if (isremove)
            {
                Component component = components.Find(c => c.Name == comName);
                component.IsInstalled = false;
                Console.WriteLine($"Removing {comName}");

                if (component.IsInstalled && component.InstalledExplicitly)
                {
                    //check for
                }
                else if (component.IsInstalled && !component.InstalledExplicitly)
                {
                    //check for 
                }

                foreach(string d in component.Dependencies)
                {
                    RemoveComponent(components, d,false);
                }
            }


        }


        public static void List(List<Component> components)
        {
            Console.WriteLine("LIST");
            foreach(Component c in components.FindAll(com=> com.IsInstalled))
            {
                Console.WriteLine(c.Name);
            }
        }

        public static List<Component> RemoveUnusedComponent(List<Component> components)
        {
            return components.FindAll(c=> c.IsInstalled == true);
        }



    }

    public class Component
    {

        public Component(string name)
        {
            Name = name;
            IsInstalled = false;
            InstalledExplicitly = false;
            Dependencies = new List<string>();
        }
        public string Name { get; set; }
        public bool IsInstalled { get; set; }
        public bool InstalledExplicitly { get; set; }
        public List<string> Dependencies { get; set; }

    }

    public class Node
    {
        public Node(int v)
        {
            V = v;
            visited = false;
            Edges = new List<Node>();
        }
        public int V { get; set; }
        public bool visited { get; set; }
        public List<Node> Edges { get; set; }

        public void Addedge(int w)
        {
            var edge = Edges.Find(e => e.V == w);
            if(edge == null)
            {
                Edges.Add(new Node(w));
            }
        }

    }

    public class Graph
    {

        public Graph()
        {
            //_size = size;
            //g = new List<Node>(_size);
        }
        private int _size;
        private List<Node> adj = new List<Node>();


        public void AddEdge(int v,int w)
        {
            Node n = adj.Find(gg => gg.V == v);
            if (n == null)
            {
                n = new Node(v);
                adj.Add(n);
            }
            n.Addedge(w);
            
        }

        public void ResetAdjList()
        {
            foreach(var n in adj)
            {
                n.visited = false;
            }
        }

        public void DFS(int start)
        {
            Console.WriteLine("DFS Traversal:-");
            Dodfs(start);
            Console.WriteLine();
        }
        private void Dodfs(int start)
        {

            Node item = adj.Find(gg => gg.V == start);
            if (!item.visited)
            {
                item.visited = true;
                Console.Write(item.V + " ");

                foreach (var n in item.Edges)
                {
                    Dodfs(n.V);
                }
            }
            
        }

        public void BFS(int start)
        {
            Console.WriteLine("BFS Traversal:-");
            Dobfs(adj.Find(a => a.V == start));

        }
        private void Dobfs(Node lst)
        {

            Queue<Node> q = new Queue<Node>();

            q.Enqueue(lst);

            while(q.Count > 0)
            {

                Node temp = q.Dequeue();
                Node item = adj.Find(a => a.V == temp.V);
                if (!item.visited)
                {
                    Console.Write(item.V + " ");
                    item.visited = true;

                    foreach(Node d in item.Edges)
                    {
                        q.Enqueue(d);
                    }

                }



            }

            Console.WriteLine();
        }

    }

}





