﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class CommandScript : IList<CommandInstance>
    {
        private List<CommandInstance> _list = new List<CommandInstance>();
        public CommandScript()
        {

        }

        public CommandInstance this[int index] 
        { 
            get => _list[index];
            set => _list[index] = value; 
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(CommandInstance item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(CommandInstance item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(CommandInstance[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CommandInstance> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(CommandInstance item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, CommandInstance item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(CommandInstance item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
