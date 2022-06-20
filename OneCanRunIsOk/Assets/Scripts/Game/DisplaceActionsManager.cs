using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class DisplaceActionsManager : MonoBehaviour
    {
        public List<DisplaceAction> curActions;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            foreach(DisplaceAction da in curActions)
            {
                da.displaceObject.transform.position = Vector3.MoveTowards(da.displaceObject.transform.position, 
                    da.destination, da.displaceSpeed * Time.deltaTime);
            }
        }
    }
}
