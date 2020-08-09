using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace UserAuthWithSaltedPassword
{
    public class EventsLogger
    {
        public static void WriteLog(string text)
        {
            try
            {
                using (EventLog log = new EventLog("Application"))
                {
                    log.Source = "Application";
                    log.WriteEntry(text, EventLogEntryType.Error, 234, (short)3);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}