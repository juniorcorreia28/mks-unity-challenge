using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToPool;
    [SerializeField]
    private List<int> amountToPool;

    /// <summary>Lista geral contendo todas as listas de objeto com pooling</summary>
    private List<List<GameObject>> pooledObjects;

    public static ObjectPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        //Inicia o objeto
        pooledObjects = new List<List<GameObject>>();

        //Distribui os clones de cada objeto na lista
        for (int i1 = 0; i1 < objectsToPool.Count; i1++)
        {
            List<GameObject> objectList = new List<GameObject>();

            GameObject tmp;

            for (int i2 = 0; i2 < amountToPool[i1]; i2++)
            {
                tmp = Instantiate(objectsToPool[i1]);
                tmp.SetActive(false);
                objectList.Add(tmp);
            }

            pooledObjects.Add(objectList);
        }
    }

    /// <summary>
    /// Pega um objeto da lista de pooling
    /// </summary>
    /// <param name="element">Elemento que representa o objeto</param>
    /// <returns></returns>
    public GameObject GetPooledObject(int element)
    {
        for (int i1 = 0; i1 < pooledObjects[element].Count; i1++)
        {
            for (int i2 = 0; i2 < amountToPool[element]; i2++)
            {
                if (!pooledObjects[element][i2].activeInHierarchy)
                {
                    return pooledObjects[element][i2];
                }
            }
        }

        return null;
    }
}
