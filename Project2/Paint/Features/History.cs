using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    public class History
    {
        private Stack<Command> _history = new Stack<Command>();

        public void push(Command c)
        {
            _history.Push(c);
        }

        public Command? pop()
        {
            if (_history.Count == 0)
                return null;
            return _history.Pop();
        }

        public void clear()
        {
            _history.Clear();
        }

        public int count()
        {
            return _history.Count;
        }
    }
}
