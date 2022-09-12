using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //when object exit the trigger, put it to the assigned layer and sorting layers
    //used in the stair objects for player to travel between layers
    public class LayerTrigger : MonoBehaviour
    {
        public string layer;
        public string sortingLayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            other.gameObject.layer = LayerMask.NameToLayer(layer);

            if (other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName == sortingLayer) return;
            
            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
            SpriteRenderer[] srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach ( SpriteRenderer sr in srs)
            {
                sr.sortingLayerName = sortingLayer;
            }
            
            // 누가 sortingLayer가 뭘로 바뀌었는지 보고
            C_ChangeSortLayer layerChangePacket = new C_ChangeSortLayer();
            layerChangePacket.ObjectId = other.GetComponent<BaseController>().Id;
            layerChangePacket.LayerName = sortingLayer;
            Managers.Network.Send(layerChangePacket);
        }

    }
}
