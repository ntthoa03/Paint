
using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    internal class Draw : Command
    {
        public Draw(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            Backup(); 
            _app._shapes.Add((IShape)_app._preview);
            
            return true; 
        }
    }
}
