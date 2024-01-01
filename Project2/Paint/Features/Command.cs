
using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    public abstract class Command
    {
        protected MainWindow _app;
        public List<IShape> _backup;
        public static History _undo = new History();
        public static History _redo = new History();

        public Command(MainWindow app)
        {
            this._app = app;
        }

       
        public abstract bool Action();

        public static void actionCommand(Command c)
        {
            if (c.Action())
            {
                _redo.clear(); 
                _undo.push(c);
            }
        } 

        public void Backup()
        {
            _backup = new List<IShape>();
            foreach (var shape in _app._shapes)
            {
                _backup.Add((IShape)shape.deepCopy());
            }
        }
    }
}
