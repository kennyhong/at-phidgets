using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListDictionary
{
    private Dictionary<int, List<int>> internalDictionary = new Dictionary<int, List<int>>();

    public void Add(int key, int value)
    {
        if (this.internalDictionary.ContainsKey(key))
        {
            List<int> list = this.internalDictionary[key];
            if (list.Contains(value) == false)
            {
                list.Add(value);
            }
        }
        else
        {
            List<int> list = new List<int>();
            list.Add(value);
            this.internalDictionary.Add(key, list);
        }
    }

    public int Count()
    {
        return internalDictionary.Count;
    }

    public string PrintResults()
    {
        string result = "";

        result = string.Format(string.Format("0: {0} | 1: {1} | 2: {2} | 3: {3} | 4: {4} | 5: {5} | 6: {6} | 7: {7}", 
            string.Join(",", internalDictionary[0].ToArray()), 
            string.Join(",", internalDictionary[1].ToArray()), 
            string.Join(",", internalDictionary[2].ToArray()), 
            string.Join(",", internalDictionary[3].ToArray()), 
            string.Join(",", internalDictionary[4].ToArray()), 
            string.Join(",", internalDictionary[5].ToArray()), 
            string.Join(",", internalDictionary[6].ToArray()), 
            string.Join(",", internalDictionary[7].ToArray())));

        return result;
    }
}
