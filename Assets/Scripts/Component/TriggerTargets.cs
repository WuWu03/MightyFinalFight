using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TriggerTargets:MonoBehaviour
{
    public List<GameObject> Targets
    {
        get;
        private set;
    }

    public void Release()
    {
        Targets.Clear();
    }

    private void Awake()
    {
        Targets = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Targets.Contains(collision.gameObject))
        {
            Targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Targets.Remove(collision.gameObject);
    }
}