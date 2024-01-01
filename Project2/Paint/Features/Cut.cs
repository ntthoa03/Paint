using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    internal class Cut : Command
    {
        public Cut(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            if(_app._chooseShape != null)
            {
                Backup();
                _app._clipBoard = _app._chooseShape;
                _app._shapes.Remove(_app._chooseShape);
                _app._chooseShape = null;
                return true;
            }
            return false;
        }
    }
}
