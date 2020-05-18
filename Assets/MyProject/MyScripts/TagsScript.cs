using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagsScript : MonoBehaviour
{

    [SerializeField]
    private List<string> tags = new List<string>();

    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    public string GetPlayerNumber()
    {
        for(int i = 0; i < tags.Count; i++)
        {
            //Debug.Log("checking " + tags[i]);
            if (tags[i].Contains("Player"))
            {
                //Debug.Log("returned " + tags[i]);
                return tags[i];
            }
        }
        return "";
    }

    public IEnumerable<string> GetTags()
    {
        return tags;
    }

    public void Rename(int index, string tagName)
    {
        tags[index] = tagName;
    }

    public string GetAtIndex(int index)
    {
        return tags[index];
    }

    public void Add(string tagName)
    {
        tags.Add(tagName);
    }

    public int Count
    {
        get { return tags.Count; }
    }

}
