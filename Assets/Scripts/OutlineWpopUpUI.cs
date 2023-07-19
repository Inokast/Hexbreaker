using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineWpopUpUI : MonoBehaviour
{
    [SerializeField] private Material blank;
    [SerializeField] private Material outlineMaterial;
    //[SerializeField] private string selectableTag = "Enemy";
    [SerializeField] public CanvasGroup worldCanvasGroup;

    [SerializeField] private GameObject storedObject;
    [SerializeField] private bool stored;

    private Transform _selection;


    public void ChangeShader(GameObject objectToShade)
    {
        if (objectToShade.layer != 3)
        {
            objectToShade.layer = 3;
            foreach (Transform child in objectToShade.GetComponentsInChildren<Transform>(true))
            {
                    child.gameObject.layer = LayerMask.NameToLayer("OutlineLayer");
                if (child.gameObject.name == "VFX_Holder")
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                if (child.gameObject.name == "vfxGraph_DripTongue")
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                if (child.gameObject.name == "vfxGraph_DripBody")
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }

            }
        }  
            
            //Renderer objectRendererN = objectToShade.GetComponent<Renderer>();
            //objectRendererN.material = outlineMaterial;
            Debug.Log("Set Stored Object to Outline");

            if (stored == true)
            {
                storedObject.layer = 0;
                foreach (Transform child in storedObject.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                //  var objectRenderS = storedObject.GetComponent<Renderer>();
                //  objectRenderS.material = blank;
                Debug.Log("Stored Object set to blank");
            }

            storedObject = objectToShade;
            stored = true;
            Debug.Log("Object Stored");
        }
        
    }

    

