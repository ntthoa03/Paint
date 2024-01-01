using MyLib;
using Paint.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    public class Undo : Command
    {
        public Undo(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            Command? c = _undo.pop();
            if (c != null)
            {
                Backup();
                _redo.push(this);
                _app._shapes = new List<IShape>(c._backup);
            }
            return false;
        }
    }
}
