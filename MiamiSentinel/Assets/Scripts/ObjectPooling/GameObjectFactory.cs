using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameObjectFactory<T> : ScriptableObject where T : MonoBehaviour
{
    protected Scene scene;
    protected List<T>[] pools;
    protected List<T> prefabs;

    private void Awake()
    {
        prefabs = new List<T>();
    }

    protected T CreateGameObjectInstance(int index)
    {
        T instance;
        List<T> pool = pools[index];
        int lastIndex = pool.Count - 1;
        if (lastIndex >= 0)
        {
            instance = pool[lastIndex];
            pool.RemoveAt(lastIndex);
        }
        else
        {
            instance = Instantiate(prefabs[index]);
        }
        instance.gameObject.SetActive(true);

        MoveToFactoryScene(instance.gameObject);
        return instance;

    }

    protected void MoveToFactoryScene(GameObject go)
    {
        if (!scene.isLoaded)
        {
            if (Application.isEditor)
            {
                scene = SceneManager.GetSceneByName(name);
                if (!scene.isLoaded)
                {
                    scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                scene = SceneManager.CreateScene(name);
            }
        }
        SceneManager.MoveGameObjectToScene(go, scene);
    }

    public void Unload()
    {
        Debug.Log(name + " has been disabled");
        SceneManager.UnloadSceneAsync(name);
        foreach (var pool in pools)
        {
            pool.Clear();

        }
        pools = null;
    }
}