
using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Features
{
    internal class Paste : Command
    {
        public Paste(MainWindow app) : base(app)
        {
        }

        public override bool Action()
        {
            Backup();
            if(_app._clipBoard != null)
            {
                Backup();
                // Tạo một bản sao mới cho mỗi lần paste
                IShape pasteShape = _app._clipBoard.deepCopy();
                pasteShape.pasteShape(_app._newStartPoint, _app._clipBoard);
                _app._shapes.Add(pasteShape);
                Backup();
                return true;
            }
            return false;
        }
    }
}
