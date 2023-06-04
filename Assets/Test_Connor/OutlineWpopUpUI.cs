using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineWpopUpUI : MonoBehaviour
{
    [SerializeField] private Material blank;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private string selectableTag = "Enemy";
    [SerializeField] public CanvasGroup worldCanvasGroup;

   
    private Transform _selection;

    // Update is called once per frame
    void Update()
    {
        if (_selection !=null) //reset
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = blank;
            _selection = null;
            worldCanvasGroup.alpha =0;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //on hit
        {
            var selection = hit.transform;
            if (selection.CompareTag(selectableTag))
            {
                
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = outlineMaterial;
                    worldCanvasGroup.alpha =1 ;
                }
                _selection = selection;
            }
            
        }
        
    }
}
