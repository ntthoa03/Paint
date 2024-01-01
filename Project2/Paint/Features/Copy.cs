
using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    internal class Copy : Command
    {
        public Copy(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            if(_app._chooseShape != null)
            {
                _app._clipBoard = (IShape)_app._chooseShape.deepCopy();
            }
            return false;
        }
    }
}
