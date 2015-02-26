using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBengine
{
    public class Librarian
    {
        int[] lightstatus = { 0, 0, 0, 0 }; //Used for testing purposes, will be replaced later with XML or SQL
        public Librarian()
        {

        }
        public int getStatus(int id)
        {
            if (id >= lightstatus.Length)
            {
                return -1;
            }
            else
            {
                return lightstatus[id];
            }
        }
        public int setStatus(int id, int value)
        {
            if (id >= lightstatus.Length)
            {
                return -1;
            }
            else
            {
                lightstatus[id] = value;
                return 0;
            }
        }
    }
}
