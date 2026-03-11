using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Inventories
{
    public class Inventory : IEnumerable<Item>
    {
        public event Action<Item, Vector2Int> OnAdded;
        public event Action<Item, Vector2Int> OnRemoved;
        public event Action<Item, Vector2Int> OnMoved;
        public event Action OnCleared;

        public int Width => inventorySize.x;
        public int Height => inventorySize.y;
        public int Count => idToItem.Count;
        
        private readonly Vector2Int inventorySize;
        private readonly int[,] internalArray;
        private readonly Dictionary<int, KeyValuePair<Item, Vector2Int>> idToItem = new ();

        private readonly int noItemInCellId = -1;
        private readonly Vector2Int noItemInCellPosition = Vector2Int.zero;

        public Inventory(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"Inventory width should be greater than 0, current value: {width}");
            }
            
            if (height <= 0)
            {
                throw new ArgumentException($"Inventory height should be greater than 0, current value: {height}");
            }

            inventorySize = new Vector2Int(width, height);
            internalArray = new int[width, height];
            for (int i = 0; i < inventorySize.x; ++i)
            {
                for (int j = 0; j < inventorySize.y; ++j)
                {
                    internalArray[i, j] = noItemInCellId;
                }
            }
        }

        public Inventory(
            int width,
            int height,
            params KeyValuePair<Item, Vector2Int>[] items
        ) : this(width, height)
        {
            if (items == null)
            {
                throw new ArgumentNullException($"items cannot be null");
            }
            
            foreach (var (item, position) in items)
            {
                AddItem(item, position);
            }

            
        }

        public Inventory(
            int width,
            int height,
            params Item[] items
        ) : this(width, height)
        {
            if (items == null)
            {
                throw new ArgumentNullException($"items cannot be null");
            }
            
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<KeyValuePair<Item, Vector2Int>> items
        ) : this(width, height)
        {
            if (items == null)
            {
                throw new ArgumentNullException($"items cannot be null");
            }
            
            foreach (var (item, position) in items)
            {
                AddItem(item, position);
            }
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<Item> items
        ) : this(width, height)
        {
            if (items == null)
            {
                throw new ArgumentNullException($"items cannot be null");
            }
            
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        /// <summary>
        /// Creates new inventory 
        /// </summary>
        public Inventory(Inventory inventory) : this(inventory.Width, inventory.Height)
        {
            idToItem.Clear();
            foreach (var (id, (item, startPosition)) in inventory.idToItem)
            {
                idToItem.Add(id, new KeyValuePair<Item, Vector2Int>(item, startPosition));
            }
        }

        /// <summary>
        /// Checks for adding an item on a specified position
        /// </summary>
        public bool CanAddItem(Item item, Vector2Int position)
        {
            return CanAddItem(item, position.x, position.y);
        }

        public bool CanAddItem(Item item, int startX, int startY)
        {
            try
            {
                CheckItemValidity(item);

                if (Contains(item))
                {
                    return false;
                }
                
                if (!CheckPositionInInventoryWithExceptions(startX, startY) ||
                    !CheckPositionInInventory(startX + item.Size.x - 1,startY + item.Size.y - 1)) 
                {
                    return false;
                }
            }
            catch (ArgumentNullException e)
            {
                return false;
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            
            
            if (!IsFreeSpace(startX , startY , startX + item.Size.x, startY + item.Size.y))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds an item on a specified position
        /// </summary>
        public bool AddItem(Item item, Vector2Int position)
        {
            return AddItem(item, position.x, position.y);
        }

        public bool AddItem(Item item, int startX, int startY)
        {
            if (!CanAddItem(item, startX, startY)) 
            {
                return false;
            }

            AddItemInternally(item, startX, startY);
            OnAdded?.Invoke(item, new Vector2Int(startX, startY));
            return true;
        }

        /// <summary>
        /// Checks for adding an item on a free position
        /// </summary>
        public bool CanAddItem(Item item)
        {
            return CanAddItem(item, out var _);
        }

        private bool CanAddItem(Item item, out Vector2Int freePosition)
        {
            freePosition = noItemInCellPosition;
            try
            {
                CheckItemValidity(item);
            }
            catch (ArgumentNullException e)
            {
                return false;
            }

            if (Contains(item))
            {
                return false;
            }

            return FindFreePosition(item, out freePosition);
        }

        /// <summary>
        /// Adds an item on a free position
        /// </summary>
        public bool AddItem(Item item)
        {
            if (!CanAddItem(item, out var freePosition))
            {
                return false;
            }

            AddItemInternally(item, freePosition.x, freePosition.y);
            OnAdded?.Invoke(item, freePosition);
            return true;
        }

        /// <summary>
        /// Returns a free position for a specified item
        /// </summary>
        public bool FindFreePosition(Item item, out Vector2Int position)
        {
            CheckItemValidity(item);
            return FindFreePosition(item.Size, out position);
        }

        public bool FindFreePosition(Vector2Int size, out Vector2Int position)
        {
            return FindFreePosition(size.x, size.y, out position);
        }

        public bool FindFreePosition(int sizeX, int sizeY, out Vector2Int position)
        {
            if (sizeX <= 0 || sizeY <= 0)
            {
                throw new ArgumentException("Size should be positive value");
            }
            position = noItemInCellPosition;
            int startX = 0, startY = 0;
            int lastX = Width - sizeX, lastY = Height - sizeY;
            while (startY <= lastY)
            {
                startX = 0;
                while (startX <= lastX)
                {
                    if (IsFreeSpace(startX, startY, startX + sizeX, startY + sizeY,
                            out int x,
                            out int y))
                    {
                        position.x = startX;
                        position.y = startY;
                        return true;
                    }

                    startX = x + 1;
                }

                startY++;
            }
            return false;
        }
        
        private void AddItemInternally(Item item, int startX, int startY)
        {
            for (int i = startX; i < startX + item.Size.x; ++i)
            {
                for (int j = startY; j < startY + item.Size.y; ++j)
                {
                    internalArray[i, j] = item.Id;
                }
            }

            idToItem[item.Id] =
                new KeyValuePair<Item, Vector2Int>(item, new Vector2Int(startX, startY));
        }

        private bool IsFreeSpace(int startX, int startY, int endX, int endY, out int firstOccupiedX, out int firstOccupiedY)
        {
            firstOccupiedX = -1;
            firstOccupiedY = -1;
            for (int i = startX; i < endX; ++i)
            {
                for (int j = startY; j < endY; ++j)
                {
                    if (IsOccupied(i, j))
                    {
                        firstOccupiedX = i;
                        firstOccupiedY = j;
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsFreeSpace(int startX, int startY, int endX, int endY)
        {
            return IsFreeSpace(startX, startY, endX, endY, out int _, out int _);
        }

        /// <summary>
        /// Checks if the specified element exists
        /// </summary>
        public bool Contains(Item item)
        {
            try
            {
                CheckItemValidity(item);
                return idToItem.ContainsKey(item.Id);
            }
            catch (ArgumentNullException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified position is occupied
        /// </summary>
        public bool IsOccupied(Vector2Int position)
        {
            return IsOccupied(position.x, position.y);
        }

        public bool IsOccupied(int x, int y)
        {
            return internalArray[x, y] != noItemInCellId;
        }

        /// <summary>
        /// Checks if the specified position is free
        /// </summary>
        public bool IsFree(Vector2Int position)
        {
            return !IsOccupied(position);
        }

        public bool IsFree(int x, int y)
        {
            return !IsOccupied(x, y);
        }

        /// <summary>
        /// Removes specified item
        /// </summary>
        public bool RemoveItem(Item item)
        {
            return RemoveItem(item, out var _);
        }

        public bool RemoveItem(Item item, out Vector2Int position)
        {
            position = Vector2Int.zero;
            if (!Contains(item))
            {
                return false;
            }

            position = idToItem[item.Id].Value;
            for (int i = position.x; i <= position.x + item.Size.x; ++i)
            {
                for (int j = position.y; j <= position.y + item.Size.y; ++j)
                {
                    internalArray[i, j] = -1;
                }
            }

            idToItem.Remove(item.Id);
            OnRemoved?.Invoke(item, position);
            return true;
        }

        /// <summary>
        /// Returns an item at specified position 
        /// </summary>
        public Item GetItem(Vector2Int position)
        {
            return GetItem(position.x, position.y);
        }

        public Item GetItem(int x, int y)
        {
            if (!CheckPositionInInventoryWithExceptions(x, y))
            {
                return null;
            }
            TryGetItem(x, y, out var item);
            return item;
        }

        public bool TryGetItem(Vector2Int position, out Item item)
        {
            return TryGetItem(position.x, position.y, out item);
        }

        public bool TryGetItem(int x, int y, out Item item)
        {
            item = null;
            try
            {
                if (!CheckPositionInInventoryWithExceptions(x, y))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            
            if (!IsOccupied(x, y))
            {
                return false;
            }

            int id = internalArray[x, y];
            item = idToItem[id].Key;
            return true;
        }

        /// <summary>
        /// Returns positions of a specified item 
        /// </summary>
        public Vector2Int[] GetPositions(Item item)
        {
            if (!TryGetPositions(item, out var positions))
            {
                throw new KeyNotFoundException(
                    $"Inventory does not contain item {item.Name}");
            }
            return positions;
        }

        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        {
            positions = null;
            if (!Contains(item))
            {
                return false;
            }

            positions = new Vector2Int[item.Size.x * item.Size.y];
            var startPosition = idToItem[item.Id].Value;
            int indexInPositions = 0;
            for (int i = startPosition.x; i < startPosition.x + item.Size.x; ++i)
            {
                for (int j = startPosition.y; j < startPosition.y + item.Size.y; ++j)
                {
                    positions[indexInPositions].x = i;
                    positions[indexInPositions].y = j;
                    indexInPositions++;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears all items 
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < inventorySize.x; ++i)
            {
                for (int j = 0; j < inventorySize.y; ++j)
                {
                    internalArray[i, j] = noItemInCellId;
                }
            }

            if (idToItem.Count > 0)
            {
                idToItem.Clear();
                OnCleared?.Invoke();
            }
        }

        /// <summary>
        /// Returns count of items with a specified name
        /// </summary>
        public int GetItemCount(string name)
        {
            int count = 0;
            foreach (var (id, (item, startPosition)) in idToItem)
            {
                if (string.Equals(name, item.Name))
                {
                    count++;
                }
            }

            return count;
        }

        public bool MoveItem(Item item, Vector2Int position)
        {
            CheckItemValidity(item);

            if (!Contains(item))
            {
                return false;
            }

            try
            {
                if (!CheckPositionInInventoryWithExceptions(position.x, position.y) ||
                    !CheckPositionInInventory(position.x + item.Size.x - 1,
                        position.y + item.Size.y - 1))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            for (int i = position.x; i <= position.x + item.Size.x; ++i)
            {
                for (int j = position.y; j <= position.y + item.Size.y; ++j)
                {
                    if (IsOccupied(i, j ) && internalArray[i, j] != item.Id)
                    {
                        return false;
                    }
                }
            }

            var currentPosition = idToItem[item.Id].Value;
            for (int i = currentPosition.x; i <= currentPosition.x + item.Size.x; ++i)
            {
                for (int j = currentPosition.y; j <= currentPosition.y + item.Size.y; ++j)
                {
                    if (IsOccupied(i, j ) && internalArray[i, j] != item.Id)
                    {
                        return false;
                    }
                }
            }

            for (int i = position.x; i <= position.x + item.Size.x; ++i)
            {
                for (int j = position.y; j <= position.y + item.Size.y; ++j)
                {
                    internalArray[i, j] = item.Id;
                }
            }

            idToItem[item.Id] = new KeyValuePair<Item, Vector2Int>(item, position);
            OnMoved?.Invoke(item, position);
            return true;
        }

        /// <summary>
        /// Rearranges an inventory space with max free slots 
        /// </summary>
        public void OptimizeSpace()
        {
            Item[] currentItems = new Item[idToItem.Count];
            int idx = 0;
            foreach (var (id, (item, position)) in idToItem)
            {
                currentItems[idx] = item;
                idx++;
            }
            
            Array.Sort(currentItems, new InternalItemComparator());

            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    internalArray[i, j] = noItemInCellId;
                }
            }

            for (int i = 0; i < idToItem.Count; ++i)
            {
                var item = currentItems[i];
                FindFreePosition(item, out var position);
                AddItemInternally(item, position.x, position.y);
            }
        }
        
        private class InternalItemComparator : IComparer<Item>
        {
            public int Compare(Item i1, Item i2)
            {
                if (i1 == null)
                {
                    return -1;
                }

                if (i2 == null)
                {
                    return 1;
                }
                
                int s1 = i1.Size.x * i1.Size.y;
                int s2 = i2.Size.x * i2.Size.y;

                return s1 == s2 ? 0 : (s1 < s2 ? 1 : -1);
            }
        }
        

        private bool CheckPositionInInventory(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
        }

        private bool CheckPositionInInventoryWithExceptions(int x, int y)
        {
            if (x < 0)
            {
                throw new IndexOutOfRangeException(
                    $"position in inventory should be positive, current x: {x}");
            }
            
            if (x >= Width)
            {
                throw new IndexOutOfRangeException(
                    $"x position in inventory cannot be greater than its width, current x: {x}, current width: {Width}");
            }
            
            if (y < 0)
            {
                throw new IndexOutOfRangeException(
                    $"position in inventory should be positive, current y: {y}");
            }
            
            if (y >= Height)
            {
                throw new IndexOutOfRangeException(
                    $"y position in inventory cannot be greater than its height, current y: {y}, current height: {Height}");
            }

            return true;
        }
        
        private bool CheckItemValidity(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Cannot use null item");
            }
            
            if (item.Size.x <= 0)
            {
                throw new ArgumentException($"item width should be greater than 0, current value: {item.Size.x}");
            }
            
            if (item.Size.y <= 0)
            {
                throw new ArgumentException($"item height should be greater than 0, current value: {item.Size.y}");
            }

            return true;
        }

        /// <summary>
        /// Iterates by all items 
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return new InventoryEnumerator(idToItem.GetEnumerator());
        }

        /// <summary>
        /// Copies items to a specified matrix
        /// </summary>
        public void CopyTo(Item[,] matrix)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    matrix[i, j] = idToItem[internalArray[i, j]].Key;
                }
            }
        }

        /// <summary>
        /// Returns an inventory matrix in string format
        /// </summary>
        public override string ToString()
        {
            string[,] itemNames = new string[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    if (!IsOccupied(i, j))
                    {
                        itemNames[i, j] = "null";
                    }
                    else
                    {
                        itemNames[i, j] = idToItem[internalArray[i, j]].Key.Name;
                    }
                }
            }

            return itemNames.ToString();
        }

        public class InventoryEnumerator : IEnumerator<Item>
        {
            private IEnumerator<KeyValuePair<int, KeyValuePair<Item, Vector2Int>>> dictEnumerator;

            public InventoryEnumerator(IEnumerator<KeyValuePair<int, KeyValuePair<Item, Vector2Int>>>  enumerator)
            {
                dictEnumerator = enumerator;
            }
            public bool MoveNext()
            {
                return dictEnumerator.MoveNext();
            }

            public void Reset()
            {
                dictEnumerator.Reset();
            }

            public Item Current { get => dictEnumerator.Current.Value.Key; } 

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                dictEnumerator.Dispose();
            }
        }
    }
}