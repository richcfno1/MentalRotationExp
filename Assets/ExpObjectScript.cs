using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpObjectScript : MonoBehaviour
{
    public GameObject expObject1;
    public GameObject expObject2;
    public GameObject randomSourceObject;
    public List<GameObject> designedObject;

    private Vector3 expObjectPosition1;
    private Vector3 expObjectPosition2;
    private readonly List<GameObject> allCubes = new List<GameObject>();

    void Start()
    {
        expObjectPosition1 = expObject1.transform.position;
        expObjectPosition2 = expObject2.transform.position;
    }

    private void AdjustCenter(GameObject expObject, Vector3 posObject)
    {
        Vector3 finalPosition = Vector3.zero;
        foreach (Transform i in expObject.GetComponentsInChildren<Transform>())
        {
            finalPosition += i.position;
        }
        finalPosition /= expObject.GetComponentsInChildren<Transform>().Length;
        expObject.transform.position += posObject - finalPosition;
    }

    private Vector3 CreateBranch(Transform parent, Vector3 startPosition, Vector3 direction, int size)
    {
        Vector3 position = startPosition;
        for (int i = 0; i < size; i++)
        {
            position += direction;
            allCubes.Add(Instantiate(randomSourceObject, position, new Quaternion(), parent));
        }
        return position;
    }

    private void CreateHalfObject(Transform parent, Vector3 startPosition, int branchNumber)
    {
        Vector3 position = startPosition;
        for (int i = 0; i < branchNumber; i++)
        {
            Vector3 branchDirection = Vector3.zero;
            switch(Random.Range(0, 6))
            {
                case 0:
                    branchDirection.x = 1;
                    break;
                case 1:
                    branchDirection.x = -1;
                    break;
                case 2:
                    branchDirection.y = 1;
                    break;
                case 3:
                    branchDirection.y = -1;
                    break;
                case 4:
                    branchDirection.z = 1;
                    break;
                case 5:
                    branchDirection.z = -1;
                    break;
                default:
                    Debug.LogError("ERROR");
                    break;
            }
            int branchSize = Random.Range(0, 6);
            position = CreateBranch(parent, position, branchDirection, branchSize);
        }
    }

    public float CreateRandomObjects(bool same)
    {
        int branchNumber = 2;
        allCubes.Add(Instantiate(randomSourceObject, expObject1.transform, false));
        CreateHalfObject(expObject1.transform, expObject1.transform.position, branchNumber);
        CreateHalfObject(expObject1.transform, expObject1.transform.position, branchNumber);
        // If same, then copy those gameobjects into the second one
        if (same)
        {
            List<GameObject> tempCubes = new List<GameObject>();
            foreach (GameObject i in allCubes)
            {
                tempCubes.Add(Instantiate(i, expObject2.transform, false));
            }
            allCubes.AddRange(tempCubes);
        }
        else
        {
            allCubes.Add(Instantiate(randomSourceObject, expObject2.transform));
            CreateHalfObject(expObject2.transform, expObject2.transform.position, branchNumber);
            CreateHalfObject(expObject2.transform, expObject2.transform.position, branchNumber);
        }

        // Rotate them
        // Need better rotate...
        expObject1.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 180.0f),
            Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));
        expObject2.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 180.0f),
            Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));

        AdjustCenter(expObject1, expObjectPosition1);
        AdjustCenter(expObject2, expObjectPosition2);

        return (expObject1.transform.rotation.eulerAngles - expObject2.transform.rotation.eulerAngles).magnitude;
    }

    public float CreateDesignedObjects(bool same)
    {
        if (same)
        {
            int index = Random.Range(0, designedObject.Count);
            allCubes.Add(Instantiate(designedObject[index], expObject1.transform, false));
            allCubes.Add(Instantiate(designedObject[index], expObject2.transform, false));
        }
        else
        {
            int index1 = Random.Range(0, designedObject.Count);
            allCubes.Add(Instantiate(designedObject[index1], expObject1.transform, false));
            int index2 = Random.Range(0, designedObject.Count);
            while (index2 == index1)
            {
                index2 = Random.Range(0, designedObject.Count);
            }
            allCubes.Add(Instantiate(designedObject[index2], expObject2.transform, false));
        }

        // Rotate them
        // Need better rotate...
        expObject1.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 180.0f),
            Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));
        expObject2.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 180.0f),
            Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));

        AdjustCenter(expObject1, expObjectPosition1);
        AdjustCenter(expObject2, expObjectPosition2);

        return (expObject1.transform.rotation.eulerAngles - expObject2.transform.rotation.eulerAngles).magnitude;
    }

    public void ClearAllExpObjects()
    {
        foreach (GameObject i in allCubes)
        {
            Destroy(i);
        }
        allCubes.Clear();
        expObject1.transform.rotation = new Quaternion();
        expObject2.transform.rotation = new Quaternion();
    }
}
