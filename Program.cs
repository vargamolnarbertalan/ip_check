using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace ip_ellenorzo
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        private static List<string> saved_ips = new List<string>();        
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            if (db_start_siker())
            {
                bool open = false;
                StreamWriter sw = new StreamWriter("ip_checker_save.txt");
                if (private_changed())
                {
                    ShowWindow(handle, SW_SHOW);
                    open = true;                    
                }

                Console.WriteLine();

                if (public_changed())
                {
                    if (open)
                    {

                    }
                    else
                    {
                        ShowWindow(handle, SW_SHOW);
                        open = true;                        
                    }
                }            
                

                sw.WriteLine(saved_ips[0]);
                
                sw.WriteLine(saved_ips[1]);
                
                sw.Close();

                Console.WriteLine();

                Console.WriteLine("Valtozasok elmentve, az ablak bezarhato.");


                if (open)
                {
                    Console.ReadKey();
                }
            }
            
        }
               

        private static bool db_start_siker() {      
            try
            {
                StreamReader db_con = new StreamReader("ip_checker_save.txt");
                while (!db_con.EndOfStream)
                {
                    saved_ips.Add(db_con.ReadLine());
                }
                db_con.Close();                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;     
            }
            
            return true;
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Nem talalhato IPv4 cim a halozaton!");
        }

        private static bool private_changed() {
            if (GetLocalIPAddress() == saved_ips[0])
            {
                //nem valtozott
                return false;
            }
            else
            {
                //valtozott
                Console.WriteLine("A lokalis IP elozo inditas ota megvaltozott!");
                Console.WriteLine("Uj lokalis ip cim: "+ GetLocalIPAddress());
                saved_ips[0] = GetLocalIPAddress();
                return true;
            }
        }

        private static string GetPublicIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream_x = new StreamReader(response.GetResponseStream()))
            {
                address = stream_x.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);            
            return address;
        }

        private static bool public_changed()
        {
            if (GetPublicIPAddress() == saved_ips[1])
            {
                //nem valtozott
                return false;
            }
            else
            {
                //valtozott
                Console.WriteLine("A nyilvanos IP elozo inditas ota megvaltozott!");
                Console.WriteLine("Uj nyilvanos ip cim: " + GetPublicIPAddress());
                saved_ips[1] = GetPublicIPAddress();
                return true;
            }
        }



    }
}
