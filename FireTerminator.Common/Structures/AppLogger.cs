using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireTerminator.Common.Structures
{
    public sealed class AppLogger
    {
        private static Queue<string> Messages = new Queue<string>();
        private static object syncObj = new object();
        public static int Count
        {
            get { return Messages.Count; }
        }
        public static void Write(string msg)
        {
            lock (syncObj)
                Messages.Enqueue(msg);
        }
        public static string Pick()
        {
            if (Messages.Count > 0)
            {
                string msg = "";
                lock (syncObj)
                    msg = Messages.Dequeue();
                return msg;
            }
            return null;
        }
    }
}
