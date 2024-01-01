using MyLib;
using Paint.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    public class Redo : Command
    {
        public Redo(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            Command? c = _redo.pop();
            if (c != null)
            {
                Backup();
                _undo.push(this);
                _app._shapes = new List<IShape>(c._backup);
            }
            return false;
        }
    }
}
