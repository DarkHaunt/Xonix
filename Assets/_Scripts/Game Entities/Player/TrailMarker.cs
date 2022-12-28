using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Trail
{
    /// <summary>
    /// Marks tiles as trail one
    /// </summary>
    public class TrailMarker
    {
        //private const string TrailNodeSourcePath = "";

        private readonly GridNodeSource _trailNodeSource;


        public TrailMarker(GridNodeSource trailNodeSource)
        {
            _trailNodeSource = trailNodeSource;
        }



        public void MarkNodeAsTrail(GridNode gridNode) => gridNode.SetSource(_trailNodeSource);
    } 
}
