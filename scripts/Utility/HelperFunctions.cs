using Godot;
using System;
using System.Xml.XPath;

public static class HelperFunctions
{

    /// <summary>
    ///  <para>Checks if the Node has at least one child of the Specified Type.</para>
    ///  <para> Does not Count how many children are of the specified Type</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool HasChild<T>(this Node node)
    {

        foreach(var child in node.GetChildren())
        {
            if (child is T)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// <para>Checks if a Child of the Type Exists and returns the first instance as a Node </para>
    /// <para>If the Node does not have a Child of the the Type result will be null</para>
    /// </summary>
    /// <typeparam type="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    
    public static bool TryGetFirstChild<T>(this Node node, out T result) where T : Node
    {
        result = null;
        foreach(var child in node.GetChildren())
        {
            if(child is T)
            {
                result = child as T;
                return true;
            }
        }
        return false;
    }


}
