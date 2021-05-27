using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{
    public class DateTimeArgs
    {
        public enum AMPMSpec
        {
            Unspecified,
            AM,
            PM
        };
        public enum TimeZoneSpec
        {
            Unspecified,
            Zulu,
            Ahead,
            Behind
        }

        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
        public int FracSecond;
        public AMPMSpec AMPM;
        public TimeZoneSpec TimeZone;
        public int ZoneHour;
        public int ZoneMinute;
        public DateTimeArgs(int year = -1, int month = -1, int day = -1, int hour = -1, int minute = -1, int second = -1, int fracSecond = -1, 
            AMPMSpec ampm = AMPMSpec.Unspecified, TimeZoneSpec timeZone = TimeZoneSpec.Unspecified, int zoneHour = -1, int zoneMinute = -1)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            FracSecond = fracSecond;
            AMPM = ampm;
            TimeZone = timeZone;
            ZoneHour = zoneHour;
            ZoneMinute = zoneMinute;
        }
        public static DateTimeArgs Union(DateTimeArgs a, DateTimeArgs b)
        {
            return new DateTimeArgs()
            {
                Year = a.Year < 0 ? b.Year : a.Year,
                Month = a.Month < 0 ? b.Month : a.Month,
                Day = a.Day < 0 ? b.Day : a.Day,
                Hour = a.Hour < 0 ? b.Hour : a.Hour,
                Minute = a.Minute < 0 ? b.Minute : a.Minute,
                Second = a.Second < 0 ? b.Second : a.Second,
                FracSecond = a.FracSecond < 0 ? b.FracSecond : a.FracSecond,
                AMPM = a.AMPM == AMPMSpec.Unspecified ? b.AMPM : a.AMPM,
                TimeZone = a.TimeZone == TimeZoneSpec.Unspecified ? b.TimeZone : a.TimeZone,
                ZoneHour = a.ZoneHour < 0 ? b.ZoneHour : a.ZoneHour,
                ZoneMinute = a.ZoneMinute < 0 ? b.ZoneMinute : a.ZoneMinute
            };
        }
        public DateTimeArgs Union(DateTimeArgs a)
        {
            Year = a.Year < 0 ? Year : a.Year;
            Month = a.Month < 0 ? Month : a.Month;
            Day = a.Day < 0 ? Day : a.Day;
            Hour = a.Hour < 0 ? Hour : a.Hour;
            Minute = a.Minute < 0 ? Minute : a.Minute;
            Second = a.Second < 0 ? Second : a.Second;
            FracSecond = a.FracSecond < 0 ? FracSecond : a.FracSecond;
            AMPM = a.AMPM == AMPMSpec.Unspecified ? AMPM : a.AMPM;
            TimeZone = a.TimeZone == TimeZoneSpec.Unspecified ? TimeZone : a.TimeZone;
            ZoneHour = a.ZoneHour < 0 ? ZoneHour : a.ZoneHour;
            ZoneMinute = a.ZoneMinute < 0 ? ZoneMinute : a.ZoneMinute;
            return new DateTimeArgs(Year, Month, Day, Hour, Minute, Second, FracSecond, AMPM, TimeZone, ZoneHour, ZoneMinute);
        }
        public static DateTimeArgs Normalize(DateTimeArgs a)
        {
            DateTime t = DateTime.Now;
            return new DateTimeArgs()
            { 
                Year = a.Year < 0 ? t.Year : a.Year,
                Month = a.Month < 0 ? t.Month : a.Month,
                Day = a.Day < 0 ? a.Month > 0 ? 1 : t.Day : a.Day,
                Hour = a.Hour < 0 ? 0 : a.Hour + (a.AMPM == AMPMSpec.PM ? 12 : 0),
                Minute = a.Minute < 0 ? 0 : a.Minute,
                Second = a.Second < 0 ? 0 : a.Second,
                FracSecond = a.FracSecond < 0 ? 0 : a.FracSecond,
                AMPM = AMPMSpec.Unspecified,
                TimeZone = a.TimeZone,
                ZoneHour = (a.ZoneHour < 0 || a.TimeZone == TimeZoneSpec.Unspecified || a.TimeZone == TimeZoneSpec.Zulu) ? 0 : a.ZoneHour,
                ZoneMinute = (a.ZoneMinute < 0 || a.TimeZone == TimeZoneSpec.Unspecified || a.TimeZone == TimeZoneSpec.Zulu) ? 0 : a.ZoneMinute
            };
        }
        public static DateTime CreateDateTime(DateTimeArgs args)
        {
            Validate(args);
            DateTimeKind dtk = args.TimeZone == TimeZoneSpec.Unspecified ? DateTimeKind.Unspecified : DateTimeKind.Utc;
            DateTime dt = new DateTime(args.Year, args.Month, args.Day, args.Hour, args.Minute, args.Second, dtk);
            if(args.FracSecond > 0)
            {
                dt = new DateTime(dt.Ticks + args.FracSecond, dtk);
            }
            if(args.TimeZone == TimeZoneSpec.Ahead || args.TimeZone == TimeZoneSpec.Behind)
            {
                TimeSpan ts = new TimeSpan(args.ZoneHour, args.ZoneMinute, 0);
                dt += args.TimeZone == TimeZoneSpec.Ahead ? ts : -ts;
            }
            return dt;
        }
        public static void Validate(DateTimeArgs args)
        {
            if (args.Year < 100) throw new ArgumentException("Invalid year");
            if (args.Month < 1 || args.Month > 12) throw new ArgumentException("Invalid month");
            if (args.Day < 1 || args.Day > 31) throw new ArgumentException("Invalid day");
            if (args.Hour < 0 || args.Hour > 23) throw new ArgumentException("Invalid hour");
            if (args.Minute < 0 || args.Minute > 59) throw new ArgumentException("Invalid minute");
            if (args.Second < 0 || args.Second > 59) throw new ArgumentException("Invalid second");
            if (args.Minute < 0 || args.FracSecond > 999) throw new ArgumentException("Invalid fractional seconds");
            if (args.AMPM != AMPMSpec.Unspecified) throw new ArgumentException("Unnormalized date/time args");
            if (args.ZoneHour < 0 || (args.ZoneHour > 0 && (args.TimeZone == TimeZoneSpec.Zulu || args.TimeZone == TimeZoneSpec.Unspecified)))
                throw new ArgumentException("Invalid time zone hour");
            if (args.ZoneMinute < 0 || (args.ZoneMinute > 0 && (args.TimeZone == TimeZoneSpec.Zulu || args.TimeZone == TimeZoneSpec.Unspecified)))
                throw new ArgumentException("Invalid time zone minute");
        }
    }
}
