using UnityEngine;

public class TypeOfTest : MonoBehaviour
{
    public abstract class Mammal { }

    public class Human : Mammal
    {
        public string Name { get; set; }

        public Human(string name)
        {
            Name = name;
        }
    }

    private Human me;

    // Start is called before the first frame update
    void Start()
    {
        me = new Human("Richard");

        // Is the type of me assignable from type of Mammal
        Debug.Log((typeof(Mammal).IsAssignableFrom(me.GetType())) ? "True" : "False");

        // Is the type of me assignable from type of Human
        Debug.Log((typeof(Human).IsAssignableFrom(me.GetType())) ? "True" : "False");

        // Is me an instance of type Mammal
        Debug.Log(typeof(Mammal).IsInstanceOfType(me) ? "True" : "False");

        // Is me an instance of type Human
        Debug.Log(typeof(Human).IsInstanceOfType(me) ? "True" : "False");

        // Is me Mammal
        Debug.Log(me is Mammal ? "True" : "False");

        // Is me Human
        Debug.Log(me is Human ? "True" : "False");
    }
}