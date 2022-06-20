using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class DisplaceActionsManager : MonoBehaviour
    {
        public List<DisplaceAction> curActions=new List<DisplaceAction>();

        // Update is called once per frame
        void Update()
        {
            foreach(DisplaceAction da in curActions)
            {
                da.displaceObject.transform.position = Vector3.MoveTowards(da.displaceObject.transform.position, 
                    da.destination, da.displaceSpeed * Time.deltaTime);
            }
            UpdateList(Time.time);
        }

        void UpdateList(float time)
        {
            List<DisplaceAction> DeletedActions = new List<DisplaceAction>();
            for(int i = 0; i < curActions.Count; i++)
            {
                if (curActions[i].endTime < time)
                {
                    DeletedActions.Add(curActions[i]);
                    //Debug.Log("delete!");
                }
            }
            if (DeletedActions.Count > 0)
            {
                for(int i = 0; i < DeletedActions.Count; i++)
                {
                    curActions.Remove(DeletedActions[i]);
                }
            }
        }

        public void addAction(DisplaceAction da)
        {
            curActions.Add(da);
        }
    }
}
