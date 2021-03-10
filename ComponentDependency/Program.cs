using System;
using System.Collections.Generic;
using System.Text;


namespace ComponentDependency
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
            foreach(string s in inputs)
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
