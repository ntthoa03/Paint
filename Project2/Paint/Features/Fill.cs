using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    internal class Fill : Command
    {
        public Fill(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            if (_app._chooseShape != null)
            {
                Backup();
                _app._chooseShape.Background = _app._currentColor; 


                return true;
            }
            return false;
        }
    }
}
