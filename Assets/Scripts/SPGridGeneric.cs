using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SpatialPartition
{
    // I will not go into details in this file as it is similar to what you already
    // did on your previous assignments, but I saw it working, and the choice of
    // having a grid is a good choice. I think you have issues with the size
    // size of the grid elements from what I could see but overall very good choice.
    [Serializable]
    public class SPGridGeneric<TItem>
    {
        public Vector2 BoxSize;
        public Vector2 Offset;
        
        private Dictionary<Vector2Int, List<TItem>> _boxes = new();
        private List<TItem> _universalItems = new();

        public SPGridGeneric(Vector2 boxSize) => BoxSize = boxSize;
        public SPGridGeneric(float boxWidth, float boxHeight) => BoxSize = new(boxWidth, boxHeight);

        public void UpdateBoxes(IEnumerable<TItem> items, Func<TItem, Vector2> positionFunc)
        {
            // Empty old boxes
            _boxes.Clear();
            _universalItems.Clear();

            foreach (var item in items)
            {
                // Get position
                var itemPosition = positionFunc(item);
                var boxPosition = GetBoxCoords(itemPosition);

                // Add new box if needed
                if (!_boxes.ContainsKey(boxPosition)) _boxes[boxPosition] = new List<TItem>();

                // Add to box
                _boxes[boxPosition].Add(item);
            }
        }
        
        public void UpdateBoxes(IEnumerable<TItem> items, Func<TItem, Vector2> positionFunc, Func<TItem, bool> universalCheck)
        {
            // Empty old boxes
            _boxes.Clear();
            _universalItems.Clear();

            foreach (var item in items)
            {
                if (universalCheck(item))
                {
                    _universalItems.Add(item);
                    continue;
                }
                
                // Get position
                var itemPosition = positionFunc(item);
                var boxPosition = GetBoxCoords(itemPosition);

                // Add new box if needed
                if (!_boxes.ContainsKey(boxPosition)) _boxes[boxPosition] = new List<TItem>();

                // Add to box
                _boxes[boxPosition].Add(item);
            }
        }

        public List<TItem> GetBox(Vector2Int boxPosition)
        {
            _boxes.TryGetValue(boxPosition, out var value);

            return value ?? new();
        }

        public List<TItem> GetBox(Vector2 worldPosition)
        {
            _boxes.TryGetValue(GetBoxCoords(worldPosition), out var value);

            return value ?? new();
        }

        public Vector2Int GetBoxCoords(Vector2 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt((worldPosition.x + Offset.x) / BoxSize.x),
                Mathf.FloorToInt((worldPosition.y + Offset.y) / BoxSize.y)
            );
        }

        public List<TItem> GetLargerNeighborhood(Vector2 position, int radius)
        {
            List<TItem> neighbors = new();
            Vector2Int centerBoxCoords = GetBoxCoords(position);
            
            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    var delta = new Vector2Int(
                        x - radius,
                        y - radius
                    );
                    
                    neighbors.AddRange(GetBox(centerBoxCoords + delta));
                }
            }
            
            neighbors.AddRange(_universalItems);

            return neighbors;
        }

        public List<TItem> GetNeighbors(Vector2 position)
        {
            Vector2Int[] deltas =
            {
                // Cardinals
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,

                // Corners
                Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
                Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
            };

            List<TItem> neighbors = new();
            
            // Add center
            Vector2Int centerBoxCoords = GetBoxCoords(position);
            neighbors.AddRange(GetBox(centerBoxCoords));

            foreach (var delta in deltas)
                neighbors.AddRange(GetBox(centerBoxCoords + delta));
            
            // Add universal items
            neighbors.AddRange(_universalItems);

            return neighbors;
        }

        public void DrawGizmos()
        {
            // Draw any boxes with contents
            foreach (var boxDef in _boxes.Keys)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube((boxDef * BoxSize) + BoxSize / 2f - Offset, BoxSize);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube((boxDef * BoxSize) + BoxSize / 2f - Offset, BoxSize * 0.9f);
            }
        }
    }
}