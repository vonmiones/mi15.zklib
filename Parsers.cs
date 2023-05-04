using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mi15libraries
{
    public static class ParserHash
    {
        public static string ToMD5Hash(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
    public class TapClass
    {
        public string IP { get; set; }
        public string UserID { get; set; }
        public string IsInvalid { get; set; }
        public string State { get; set; }
        public string VerifyStyle { get; set; }
        public string DateTime { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Seconds { get; set; }

        public override string ToString()
        {
            string hash = ($"IP={IP},UserID:{UserID},IsInvalid:{IsInvalid},State:{State},VerifyStyle:{VerifyStyle},DateTime:{DateTime},Year:{Year},Month:{Month},Day:{Day},Hour:{Hour},Minute:{Minute},Seconds:{Seconds}").ToMD5Hash();
            return $"object={hash},IP={IP},UserID:{UserID},IsInvalid:{IsInvalid},State:{State},VerifyStyle:{VerifyStyle},DateTime:{DateTime},Year:{Year},Month:{Month},Day:{Day},Hour:{Hour},Minute:{Minute},Seconds:{Seconds}";
        }
    }
    public class Recursion
    {
        static string Implode(object obj, string separator)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            string[] values = new string[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                object value = properties[i].GetValue(obj, null);
                values[i] = value?.ToString() ?? "";
            }

            return string.Join(separator, values);
        }

        public static string RecursiveImplode(object obj, string separator)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            ArrayList values = new ArrayList();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);

                if (value != null)
                {
                    if (value.GetType().IsArray)
                    {
                        foreach (object child in (Array)value)
                        {
                            values.Add(RecursiveImplode(child, separator));
                        }
                    }
                    else
                    {
                        values.Add(value.ToString());
                    }
                }
            }

            return string.Join(separator, values.ToArray());
        }
    }
    public class Parsers
    {
        public string tap(string ip, string input)
        {
            dynamic result = new TapClass();

            string pattern = @"UserID=(\d+)\s+isInvalid=(\d+)\s+state=(\d+)\s+verifystyle=(\d+)\s+time=([\d-]+\s+[\d:]+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);

            if (match.Success)
            {
                // Extract the matched fields from the captured groups
                string userID = match.Groups[1].Value;
                string isInvalid = match.Groups[2].Value;
                string state = match.Groups[3].Value;
                string verifyStyle = match.Groups[4].Value;
                string Datetime = match.Groups[5].Value;

                result.IP = ip;
                result.UserID = userID;
                result.IsInvalid = isInvalid;
                result.State = state;
                result.VerifyStyle = verifyStyle;
                result.DateTime = Datetime;
                DateTime dt = DateTime.Parse(Datetime);
                result.Year = dt.Year;
                result.Month = dt.Month;
                result.Day = dt.Day;
                result.Hour = dt.Hour;
                result.Minute = dt.Minute;
                result.Seconds = dt.Second;
            }
            else
            {
                result = "invalid";
            }
            return result.ToString();
        }

    }
}
