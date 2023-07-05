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

        objectToShade.layer = 3;
        foreach (Transform child in objectToShade.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("OutlineLayer");   
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

    // Update is called once per frame
    void Update()
    {
       
     //   if (_selection !=null) //reset
      //  {
       //     var selectionRenderer = _selection.GetComponent<Renderer>();
      //      selectionRenderer.material = blank;
      //      _selection = null;
      //      worldCanvasGroup.alpha =0;
      //  }

      //  var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      //  RaycastHit hit;
      //  if (Physics.Raycast(ray, out hit)) //on hit
      //  {
        //    var selection = hit.transform;
        //    if (selection.CompareTag(selectableTag))
        //    {
           
              //  var selectionRenderer = selection.GetComponent<Renderer>();
              //  if (selectionRenderer != null)
             //   {
             //       selectionRenderer.material = outlineMaterial;
             //       worldCanvasGroup.alpha =1 ;
             //   }
             //  _selection = selection;
        //    }
            
     //   }
        
    }
}
